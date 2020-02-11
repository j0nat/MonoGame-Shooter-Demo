using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShooterEngine.Lighting.Effects;

namespace ShooterEngine.Lighting
{
    public enum FixedFilterSize
    {
        Filter2x2,
        Filter3x3,
        Filter5x5,
        Filter7x7
    }

    class LightRenderer
    {
        #region Settings
        private static readonly int[] KernelSizes = { 2, 3, 5, 7 };

        public Vector3 LightDirection;
        public Vector3 LightColor;
        public FixedFilterSize FixedFilterSize;
        public bool VisualizeCascades;
        public bool StabilizeCascades;
        public bool FilterAcrossCascades;
        public float SplitDistance0;
        public float SplitDistance1;
        public float SplitDistance2;
        public float SplitDistance3;
        public float Bias;
        public float OffsetScale;
        public WorldRenderer worldRenderer;

        public int FixedFilterKernelSize
        {
            get { return KernelSizes[(int)FixedFilterSize]; }
        }
        #endregion

        private readonly SamplerState ShadowMapSamplerState = new SamplerState
        {
            AddressU = TextureAddressMode.Clamp,
            AddressV = TextureAddressMode.Clamp,
            AddressW = TextureAddressMode.Clamp,
            Filter = TextureFilter.Linear,
            ComparisonFunction = CompareFunction.LessEqual,
            FilterMode = TextureFilterMode.Comparison
        };

        private readonly RasterizerState CreateShadowMap = new RasterizerState
        {
            CullMode = CullMode.None,
            DepthClipEnable = false
        };

        public const int NumCascades = 4;
        private const int ShadowMapSize = 2048;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly List<Mesh> _scene;
        private RenderTarget2D _shadowMap;
        private readonly float[] _cascadeSplits;
        private readonly Vector3[] _frustumCorners;
        private readonly ShadowMapEffect _shadowMapEffect;
        private readonly MeshEffect _meshEffect;
        private readonly BoundingFrustum _boundingFrustum;

        public Texture2D ShadowMap
        {
            get { return _shadowMap; }
        }

        public LightRenderer(
            GraphicsDevice graphicsDevice, ContentManager contentManager, List<Mesh> scene, Color sunLightColor, WorldRenderer worldRenderer)
        {
            _graphicsDevice = graphicsDevice;

            _scene = scene;
            _cascadeSplits = new float[4];
            _frustumCorners = new Vector3[8];

            _shadowMapEffect = new ShadowMapEffect(graphicsDevice, contentManager.Load<Effect>("effects/ShadowMap"));
            _meshEffect = new MeshEffect(graphicsDevice, contentManager.Load<Effect>("effects/Mesh"));

            _boundingFrustum = new BoundingFrustum(Matrix.Identity);

            LightDirection = Vector3.Normalize(new Vector3(5, 3, -1));
            LightColor = sunLightColor.ToVector3();
            Bias = 0.0015f;
            OffsetScale = 0.0f;

            StabilizeCascades = true;
            VisualizeCascades = false;

            SplitDistance0 = 0.05f;
            SplitDistance1 = 0.15f;
            SplitDistance2 = 0.50f;
            SplitDistance3 = 1.0f;

            this.FixedFilterSize = FixedFilterSize.Filter7x7;

            _meshEffect.sunLightDirection = new Vector3(-1, 1, 1);
            _meshEffect.sunLightColor = sunLightColor;
            _meshEffect.sunlightIntensity = 0;

            this.worldRenderer = worldRenderer;

            CreateShadowMaps();
        }

        private void CreateShadowMaps()
        {
            if (_shadowMap != null)
                _shadowMap.Dispose();

            _shadowMap = new RenderTarget2D(_graphicsDevice,
                ShadowMapSize, ShadowMapSize,
                false, SurfaceFormat.Single,
                DepthFormat.Depth24, 1,
                RenderTargetUsage.DiscardContents,
                false, NumCascades);
        }

        public Vector3 ToVector3(Vector4 value)
        {
            return new Vector3(value.X, value.Y, value.Z) / value.W;
        }

        public void AddLight(Light light)
        {
            _meshEffect.AddLight(light);
        }

        public void RenderShadowMap(GraphicsDevice graphicsDevice, Camera camera, Matrix worldMatrix)
        {
            // Set cascade split ratios.
            _cascadeSplits[0] = SplitDistance0;
            _cascadeSplits[1] = SplitDistance1;
            _cascadeSplits[2] = SplitDistance2;
            _cascadeSplits[3] = SplitDistance3;

            var globalShadowMatrix = MakeGlobalShadowMatrix(camera);
            _meshEffect.ShadowMatrix = globalShadowMatrix;

            // Render the meshes to each cascade.
            for (var cascadeIdx = 0; cascadeIdx < NumCascades; ++cascadeIdx)
            {
                // Set the shadow map as the render target
                graphicsDevice.SetRenderTarget(_shadowMap, cascadeIdx);
                graphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);

                // Get the 8 points of the view frustum in world space
                ResetViewFrustumCorners();

                var prevSplitDist = cascadeIdx == 0 ? 0.0f : _cascadeSplits[cascadeIdx - 1];
                var splitDist = _cascadeSplits[cascadeIdx];

                var invViewProj = Matrix.Invert(camera.ViewProjection);
                for (var i = 0; i < 8; ++i)
                    _frustumCorners[i] = ToVector3(Vector4.Transform(_frustumCorners[i], invViewProj));

                // Get the corners of the current cascade slice of the view frustum
                for (var i = 0; i < 4; ++i)
                {
                    var cornerRay = _frustumCorners[i + 4] - _frustumCorners[i];
                    var nearCornerRay = cornerRay * prevSplitDist;
                    var farCornerRay = cornerRay * splitDist;
                    _frustumCorners[i + 4] = _frustumCorners[i] + farCornerRay;
                    _frustumCorners[i] = _frustumCorners[i] + nearCornerRay;
                }

                // Calculate the centroid of the view frustum slice
                var frustumCenter = Vector3.Zero;
                for (var i = 0; i < 8; ++i)
                    frustumCenter = frustumCenter + _frustumCorners[i];
                frustumCenter /= 8.0f;

                // Pick the up vector to use for the light camera
                var upDir = camera.Right;

                Vector3 minExtents;
                Vector3 maxExtents;

                if (StabilizeCascades)
                {
                    // This needs to be constant for it to be stable
                    upDir = Vector3.Up;

                    // Calculate the radius of a bounding sphere surrounding the frustum corners
                    var sphereRadius = 0.0f;
                    for (var i = 0; i < 8; ++i)
                    {
                        var dist = (_frustumCorners[i] - frustumCenter).Length();
                        sphereRadius = Math.Max(sphereRadius, dist);
                    }

                    sphereRadius = (float)Math.Ceiling(sphereRadius * 16.0f) / 16.0f;

                    maxExtents = new Vector3(sphereRadius);
                    minExtents = -maxExtents;
                }
                else
                {
                    // Create a temporary view matrix for the light
                    var lightCameraPos = frustumCenter;
                    var lookAt = frustumCenter - LightDirection;
                    var lightView = Matrix.CreateLookAt(lightCameraPos, lookAt, upDir);

                    // Calculate an AABB around the frustum corners
                    var mins = new Vector3(float.MaxValue);
                    var maxes = new Vector3(float.MinValue);
                    for (var i = 0; i < 8; ++i)
                    {
                        var corner = ToVector3(Vector4.Transform(_frustumCorners[i], lightView));
                        mins = Vector3.Min(mins, corner);
                        maxes = Vector3.Max(maxes, corner);
                    }

                    minExtents = mins;
                    maxExtents = maxes;

                    // Adjust the min/max to accommodate the filtering size
                    var scale = (ShadowMapSize + FixedFilterKernelSize) / (float)ShadowMapSize;
                    minExtents.X *= scale;
                    minExtents.Y *= scale;
                    maxExtents.X *= scale;
                    maxExtents.Y *= scale;
                }

                var cascadeExtents = maxExtents - minExtents;

                // Get position of the shadow camera
                var shadowCameraPos = frustumCenter + LightDirection * -minExtents.Z;

                // Come up with a new orthographic camera for the shadow caster
                var shadowCamera = new LightCamera(
                    minExtents.X, minExtents.Y, maxExtents.X, maxExtents.Y,
                    0.0f, cascadeExtents.Z);
                shadowCamera.SetLookAt(shadowCameraPos, frustumCenter, upDir);

                if (StabilizeCascades)
                {
                    // Create the rounding matrix, by projecting the world-space origin and determining
                    // the fractional offset in texel space
                    var shadowMatrixTemp = shadowCamera.ViewProjection;
                    var shadowOrigin = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
                    shadowOrigin = Vector4.Transform(shadowOrigin, shadowMatrixTemp);
                    shadowOrigin = shadowOrigin * (ShadowMapSize / 2.0f);

                    var roundedOrigin = new Vector4(
                        (float)Math.Round(shadowOrigin.X),
                        (float)Math.Round(shadowOrigin.Y),
                        (float)Math.Round(shadowOrigin.Z),
                        (float)Math.Round(shadowOrigin.W));
                    var roundOffset = roundedOrigin - shadowOrigin;
                    roundOffset = roundOffset * (2.0f / ShadowMapSize);
                    roundOffset.Z = 0.0f;
                    roundOffset.W = 0.0f;

                    var shadowProj = shadowCamera.Projection;
                    //shadowProj.r[3] = shadowProj.r[3] + roundOffset;
                    shadowProj.M41 += roundOffset.X;
                    shadowProj.M42 += roundOffset.Y;
                    shadowProj.M43 += roundOffset.Z;
                    shadowProj.M44 += roundOffset.W;
                    shadowCamera.Projection = shadowProj;
                }

                // Draw the mesh with depth only, using the new shadow camera
                RenderDepth(graphicsDevice, shadowCamera, worldMatrix, true);

                // Apply the scale/offset matrix, which transforms from [-1,1]
                // post-projection space to [0,1] UV space
                var texScaleBias = Matrix.CreateScale(0.5f, -0.5f, 1.0f)
                    * Matrix.CreateTranslation(0.5f, 0.5f, 0.0f);
                var shadowMatrix = shadowCamera.ViewProjection;
                shadowMatrix = shadowMatrix * texScaleBias;

                // Store the split distance in terms of view space depth
                var clipDist = camera.FarZ - camera.NearZ;

                _meshEffect.CascadeSplits[cascadeIdx] = camera.NearZ + splitDist * clipDist;

                // Calculate the position of the lower corner of the cascade partition, in the UV space
                // of the first cascade partition
                var invCascadeMat = Matrix.Invert(shadowMatrix);
                var cascadeCorner = ToVector3(Vector4.Transform(Vector3.Zero, invCascadeMat));
                cascadeCorner = ToVector3(Vector4.Transform(cascadeCorner, globalShadowMatrix));

                // Do the same for the upper corner
                var otherCorner = ToVector3(Vector4.Transform(Vector3.One, invCascadeMat));
                otherCorner = ToVector3(Vector4.Transform(otherCorner, globalShadowMatrix));

                // Calculate the scale and offset
                var cascadeScale = Vector3.One / (otherCorner - cascadeCorner);
                _meshEffect.CascadeOffsets[cascadeIdx] = new Vector4(-cascadeCorner, 0.0f);
                _meshEffect.CascadeScales[cascadeIdx] = new Vector4(cascadeScale, 1.0f);
            }
        }

        public void AnimateLight(GameTime gameTime)
        {
            var deltaMilliseconds = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            var rotationY = deltaMilliseconds * 0.00025f;
            var rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, rotationY);
            var lightDirection = LightDirection;
            lightDirection = Vector3.Transform(lightDirection, rotation);
            LightDirection = lightDirection;
        }

        private void ResetViewFrustumCorners()
        {
            _frustumCorners[0] = new Vector3(-1.0f, 1.0f, 0.0f);
            _frustumCorners[1] = new Vector3(1.0f, 1.0f, 0.0f);
            _frustumCorners[2] = new Vector3(1.0f, -1.0f, 0.0f);
            _frustumCorners[3] = new Vector3(-1.0f, -1.0f, 0.0f);
            _frustumCorners[4] = new Vector3(-1.0f, 1.0f, 1.0f);
            _frustumCorners[5] = new Vector3(1.0f, 1.0f, 1.0f);
            _frustumCorners[6] = new Vector3(1.0f, -1.0f, 1.0f);
            _frustumCorners[7] = new Vector3(-1.0f, -1.0f, 1.0f);
        }

        /// <summary>
        /// Makes the "global" shadow matrix used as the reference point for the cascades.
        /// </summary>
        private Matrix MakeGlobalShadowMatrix(Camera camera)
        {
            // Get the 8 points of the view frustum in world space
            ResetViewFrustumCorners();

            var invViewProj = Matrix.Invert(camera.ViewProjection);
            var frustumCenter = Vector3.Zero;
            for (var i = 0; i < 8; i++)
            {
                _frustumCorners[i] = ToVector3(Vector4.Transform(_frustumCorners[i], invViewProj));
                frustumCenter += _frustumCorners[i];
            }

            frustumCenter /= 8.0f;

            // Pick the up vector to use for the light camera
            var upDir = camera.Right;

            // This needs to be constant for it to be stable
            if (StabilizeCascades)
                upDir = Vector3.Up;

            // Get position of the shadow camera
            var shadowCameraPos = frustumCenter + LightDirection * -0.5f;

            // Come up with a new orthographic camera for the shadow caster
            var shadowCamera = new LightCamera(-0.5f, -0.5f, 0.5f, 0.5f, 0.0f, 1.0f);
            shadowCamera.SetLookAt(shadowCameraPos, frustumCenter, upDir);

            var texScaleBias = Matrix.CreateScale(0.5f, -0.5f, 1.0f);
            texScaleBias.Translation = new Vector3(0.5f, 0.5f, 0.0f);
            return shadowCamera.ViewProjection * texScaleBias;
        }

        private void RenderDepth(GraphicsDevice graphicsDevice, LightCamera camera, Matrix worldMatrix, bool shadowRendering)
        {
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;

            graphicsDevice.RasterizerState = shadowRendering
                ? CreateShadowMap
                : RasterizerState.CullCounterClockwise;

            var worldViewProjection = worldMatrix * camera.ViewProjection;
            _shadowMapEffect.WorldViewProjection = Matrix.Identity * worldViewProjection;
            _shadowMapEffect.Apply();

            worldRenderer.DrawShadowMapWorld();
        }

        public void Render(GraphicsDevice graphicsDevice, Camera camera)
        {
            // Render scene.

            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            graphicsDevice.SamplerStates[0] = ShadowMapSamplerState;

            _meshEffect.VisualizeCascades = VisualizeCascades;
            _meshEffect.FilterAcrossCascades = FilterAcrossCascades;
            _meshEffect.FilterSize = FixedFilterSize;
            _meshEffect.Bias = Bias;
            _meshEffect.OffsetScale = OffsetScale;

            _meshEffect.ViewProjection = camera.ViewProjection;
            _meshEffect.CameraPosWS = Vector3.Zero;

            _meshEffect.ShadowMap = _shadowMap;

            _meshEffect.LightDirection = LightDirection;
            _meshEffect.LightColor = LightColor;

            _boundingFrustum.Matrix = camera.ViewProjection;

            _meshEffect.DiffuseColor = _meshEffect.sunLightColor.ToVector3();
            _meshEffect.World = Matrix.Identity;
            _meshEffect.Apply();

            worldRenderer.DrawWorld(_meshEffect);
        }
    }
}
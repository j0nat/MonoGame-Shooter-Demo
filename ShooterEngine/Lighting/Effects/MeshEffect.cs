using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ShooterEngine.Lighting.Effects
{
    public class MeshEffect
    {
        public const int NumCascades = 4;

        private readonly Effect _innerEffect;

        private readonly EffectParameter _cameraPosWSParameter;
        private readonly EffectParameter _shadowMatrixParameter;
        private readonly EffectParameter _cascadeSplitsParameter;
        private readonly EffectParameter _cascadeOffsetsParameter;
        private readonly EffectParameter _cascadeScalesParameter;
        private readonly EffectParameter _biasParameter;
        private readonly EffectParameter _offsetScaleParameter;
        private readonly EffectParameter _lightDirectionParameter;
        private readonly EffectParameter _lightColorParameter;
        private readonly EffectParameter _diffuseColorParameter;
        private readonly EffectParameter _worldParameter;
        private readonly EffectParameter _viewProjectionParameter;
        private readonly EffectParameter _shadowMapParameter;
        private readonly EffectParameter _texture;

        public bool VisualizeCascades { get; set; }
        public bool FilterAcrossCascades { get; set; }
        public FixedFilterSize FilterSize { get; set; }

        public Vector3 CameraPosWS { get; set; }
        public Matrix ShadowMatrix { get; set; }
        public float[] CascadeSplits { get; private set; }
        public Vector4[] CascadeOffsets { get; private set; }
        public Vector4[] CascadeScales { get; private set; }
        public float Bias { get; set; }
        public float OffsetScale { get; set; }
        public Vector3 LightDirection { get; set; }
        public Vector3 LightColor { get; set; }
        public Vector3 DiffuseColor { get; set; }
        public Matrix World { get; set; }
        public Matrix ViewProjection { get; set; }
        public Texture2D ShadowMap { get; set; }
        public Texture2D Texture { get; set; }
        private List<Light> lights { get; set; }

        public Vector3 sunLightDirection { get; set; }
        public Color sunLightColor { get; set; }
        public float sunlightIntensity { get; set; }

        private void SetLighting()
        {
            _innerEffect.Parameters["SunLightDirection"].SetValue(sunLightDirection);
            _innerEffect.Parameters["SunLightColor"].SetValue(sunLightColor.ToVector3());
            _innerEffect.Parameters["SunLightIntensity"].SetValue(sunlightIntensity);

            var _lightingEffectPointLightPosition = _innerEffect.Parameters["PointLightPosition"];
            var _lightingEffectPointLightColor = _innerEffect.Parameters["PointLightColor"];
            var _lightingEffectPointLightIntensity = _innerEffect.Parameters["PointLightIntensity"];

            var _lightingEffectPointLightRadius = _innerEffect.Parameters["PointLightRadius"];
            var _lightingEffectMaxLightsRendered = _innerEffect.Parameters["MaxLightsRendered"];

            int MaxLights = lights.Count;

            Vector3[] lightPositions = new Vector3[MaxLights];
            Vector3[] lightColors = new Vector3[MaxLights];
            float[] lightIntensities = new float[MaxLights];
            float[] lightRadii = new float[MaxLights];

            int count = 0;
            foreach (Light light in lights)
            {
                lightPositions[count] = light.Position;
                lightColors[count] = light.Color.ToVector3();
                lightIntensities[count] = light.Intensity;
                lightRadii[count] = light.Radii;

                count++;
            }

            _lightingEffectMaxLightsRendered.SetValue(MaxLights);
            _lightingEffectPointLightPosition.SetValue(lightPositions);
            _lightingEffectPointLightColor.SetValue(lightColors);
            _lightingEffectPointLightIntensity.SetValue(lightIntensities);
            _lightingEffectPointLightRadius.SetValue(lightRadii);
        }

        public void AddLight(Light light)
        {
            lights.Add(light);
        }

        public MeshEffect(GraphicsDevice graphicsDevice, Effect innerEffect)
        {
            _innerEffect = innerEffect;

            lights = new List<Light>();
            _cameraPosWSParameter = _innerEffect.Parameters["CameraPosWS"];
            _shadowMatrixParameter = _innerEffect.Parameters["ShadowMatrix"];
            _cascadeSplitsParameter = _innerEffect.Parameters["CascadeSplits"];
            _cascadeOffsetsParameter = _innerEffect.Parameters["CascadeOffsets"];
            _cascadeScalesParameter = _innerEffect.Parameters["CascadeScales"];
            _biasParameter = _innerEffect.Parameters["Bias"];
            _offsetScaleParameter = _innerEffect.Parameters["OffsetScale"];
            _lightDirectionParameter = _innerEffect.Parameters["LightDirection"];
            _lightColorParameter = _innerEffect.Parameters["LightColor"];
            _diffuseColorParameter = _innerEffect.Parameters["DiffuseColor"];
            _worldParameter = _innerEffect.Parameters["World"];
            _viewProjectionParameter = _innerEffect.Parameters["ViewProjection"];
            _shadowMapParameter = _innerEffect.Parameters["ShadowMap"];
            _texture = _innerEffect.Parameters["xTexture"];

            CascadeSplits = new float[NumCascades];
            CascadeOffsets = new Vector4[NumCascades];
            CascadeScales = new Vector4[NumCascades];
        }

        public void Apply()
        {
            var techniqueName = "Visualize" + VisualizeCascades + "Filter" + FilterAcrossCascades + "FilterSize" + FilterSize;
            _innerEffect.CurrentTechnique = _innerEffect.Techniques[techniqueName];

            _cameraPosWSParameter.SetValue(CameraPosWS);
            _shadowMatrixParameter.SetValue(ShadowMatrix);
            _cascadeSplitsParameter.SetValue(new Vector4(CascadeSplits[0], CascadeSplits[1], CascadeSplits[2], CascadeSplits[3]));
            _cascadeOffsetsParameter.SetValue(CascadeOffsets);
            _cascadeScalesParameter.SetValue(CascadeScales);
            _biasParameter.SetValue(Bias);
            _offsetScaleParameter.SetValue(OffsetScale);
            _lightDirectionParameter.SetValue(LightDirection);
            _lightColorParameter.SetValue(LightColor);
            _diffuseColorParameter.SetValue(DiffuseColor);
            _worldParameter.SetValue(World);
            _viewProjectionParameter.SetValue(ViewProjection);
            _shadowMapParameter.SetValue(ShadowMap);
            _texture.SetValue(Texture);

            SetLighting();

            _innerEffect.CurrentTechnique.Passes[0].Apply();
        }
    }
}
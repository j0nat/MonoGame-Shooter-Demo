using BulletSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ShooterEngine
{
    class CubeShooter : DrawableGameComponent
    {
        private Dictionary<RigidBody, Model> models = new Dictionary<RigidBody, Model>();
        private const float shootBoxInitialSpeed = 200;
        private Texture2D cubeTexture;
        private Physics physics;
        private Camera camera;
        private BasicEffect effect;

        public CubeShooter(Game game, Camera camera, Physics physics, Texture2D cubeTexture) : base (game)
        {
            this.camera = camera;
            this.physics = physics;
            this.cubeTexture = cubeTexture;
        }

        public override void Initialize()
        {
            effect = new BasicEffect(Game.GraphicsDevice);

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (KeyValuePair<RigidBody, Model> model in models)
            {
                foreach (var mesh in model.Value.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.TextureEnabled = true;
                        effect.Texture = cubeTexture;
                        effect.World = model.Key.WorldTransform;
                        effect.View = camera.View;
                        effect.Projection = camera.Projection;
                    }

                    mesh.Draw();
                }
            }

            base.Draw(gameTime);
        }

        public void Shoot(Model cubeModel, Vector3 cameraPosition, Vector3 destination)
        {
            cubeModel.Meshes[0].MeshParts[0].Effect = effect.Clone();

            CollisionShape boxShape = new BoxShape(1);

            foreach (var mesh in cubeModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }
            }

            Vector3 linVel = destination - cameraPosition;
            linVel.Normalize();

            const float mass = 1f;
            RigidBodyConstructionInfo rbInfo = new RigidBodyConstructionInfo(mass, null, boxShape);
            rbInfo.LocalInertia = boxShape.CalculateLocalInertia(mass);

            Matrix startTransform = Matrix.CreateTranslation(cameraPosition + (linVel) * 5);

            rbInfo.MotionState = new DefaultMotionState(startTransform);

            RigidBody body = new RigidBody(rbInfo);
            body.LinearFactor = new Vector3(1, 1, 1);

            body.LinearVelocity = linVel * shootBoxInitialSpeed;
            body.AngularVelocity = Vector3.Zero;
            body.ContactProcessingThreshold = 1e30f;
            body.CcdMotionThreshold = 0.0001f;
            body.CcdSweptSphereRadius = 0.4f;

            physics.World.AddRigidBody(body);

            models[body] = cubeModel;
        }
    }
}

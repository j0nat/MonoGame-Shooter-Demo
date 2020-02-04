using HLMapFileLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShooterEngine
{
    class WorldRenderer : DrawableGameComponent
    {
        private List<Mesh> meshes;
        private BasicEffect effect;
        private Camera camera;
        private Physics physics;
        private CharacterController characterController;
        private SkyDome skyDome;
        private CubeShooter cubeShooter;

        public WorldRenderer(Game game) : base(game)
        {
            this.meshes = new List<Mesh>();
        }

        public override void Initialize()
        {
            effect = new BasicEffect(GraphicsDevice);
            camera = new Camera(GraphicsDevice);
            physics = new Physics(GraphicsDevice);
            physics.Initialize();

            skyDome = new SkyDome(Game);
            Game.Components.Add(skyDome);

            characterController = new CharacterController(GraphicsDevice, this, physics, camera);
            characterController.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            List<Brush> brushes = Map.Load(@"Content\game.map", Game.Content);
            foreach (Brush brush in brushes)
            {
                meshes.Add(new Mesh(GraphicsDevice, brush));
            }

            physics.LoadWorldCollisions(meshes);

            cubeShooter = new CubeShooter(Game, camera, physics, Game.Content.Load<Texture2D>("Models\\paper_box_texture"));
            Game.Components.Add(cubeShooter);

            base.LoadContent();
        }

        public void ToggleDebug()
        {
            if (ShooterEngineGame.DEBUG)
            {
                ShooterEngineGame.DEBUG = false;
            }
            else
            {
                ShooterEngineGame.DEBUG = true;
            }
        }

        public void ShootCube(Vector3 cameraPosition, Vector3 destination)
        {
            Model cubeModel = Game.Content.Load<Model>("Models\\cube");

            cubeShooter.Shoot(cubeModel, cameraPosition, destination);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Blue);

            skyDome.Draw(camera);

            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

             foreach (Mesh mesh in meshes)
             {
                mesh.Draw(effect, camera);
             }

            if (ShooterEngineGame.DEBUG)
            {
                physics.DrawDebug(camera);
            }

            base.Draw(gameTime);
        }

        public void EndRun()
        {
            physics.ExitPhysics();
        }

        public override void Update(GameTime gameTime)
        {
            characterController.Update(gameTime);

            physics.Update(gameTime);

            base.Update(gameTime);
        }
    }
}

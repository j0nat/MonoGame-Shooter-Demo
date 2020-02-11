using HLMapFileLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShooterEngine.Lighting;
using ShooterEngine.Lighting.Effects;
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
        private Camera camera;
        private Physics physics;
        private CharacterController characterController;
        private SkyDome skyDome;
        private CubeShooter cubeShooter;
        private LightRenderer lightRenderer;
        private SpriteBatch spriteBatch;

        public WorldRenderer(Game game) : base(game)
        {
            this.meshes = new List<Mesh>();
        }

        public override void Initialize()
        {
            camera = new Camera(GraphicsDevice);
            physics = new Physics(GraphicsDevice);
            physics.Initialize();
            spriteBatch = new SpriteBatch(GraphicsDevice);
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

            lightRenderer = new LightRenderer(
                GraphicsDevice,
                Game.Content, meshes, Color.White * 0.4f, this);

            lightRenderer.AddLight(new Light(new Vector3(-2, 15, -1), Color.Orange, 2, 30));

            lightRenderer.AddLight(new Light(new Vector3(-10, 15, -1), Color.BlueViolet * 0.95f, 2, 30));
            lightRenderer.AddLight(new Light(new Vector3(-10 * 2, 15, -1), Color.Purple * 0.85f, 2, 30));
            lightRenderer.AddLight(new Light(new Vector3(-10 * 3, 15, -1), Color.Red * 0.75f, 2, 30));
            lightRenderer.AddLight(new Light(new Vector3(-10 * 4, 15, -1), Color.Green * 0.65f, 2, 30));
            lightRenderer.AddLight(new Light(new Vector3(-10 * 5, 15, -1), Color.Purple * 0.55f, 2, 30));
            lightRenderer.AddLight(new Light(new Vector3(-10 * 2, 15, -10 * 2), Color.Orange * 0.85f, 2, 30));
            lightRenderer.AddLight(new Light(new Vector3(-10 * 3, 15, -10), Color.Green * 0.75f, 2, 30));
            lightRenderer.AddLight(new Light(new Vector3(-10 * 4, 15, -10), Color.BlueViolet * 0.65f, 2, 30));
            lightRenderer.AddLight(new Light(new Vector3(-10 * 5, 15, 10), Color.Red * 0.55f, 2, 30));

            lightRenderer.AddLight(new Light(new Vector3(-40f, 27f, 39f), Color.Blue, 23, 33));

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
            lightRenderer.RenderShadowMap(GraphicsDevice, camera, Matrix.Identity);
            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.Stencil, Color.CornflowerBlue, 0, 0);

            skyDome.Draw(camera);

            lightRenderer.Render(GraphicsDevice, camera);

            if (ShooterEngineGame.DEBUG)
            {
                physics.DrawDebug(camera);
            }

            cubeShooter.Draw();

            base.Draw(gameTime);
        }

        public void DrawShadowMapWorld()
        {
            foreach (Mesh mesh in meshes)
            {
                mesh.Draw();
            }
        }

        public void DrawWorld(MeshEffect effect)
        {
            foreach (Mesh mesh in meshes)
            {
                mesh.Draw(effect);
            }
        }

        public void EndRun()
        {
            physics.ExitPhysics();
        }

        public override void Update(GameTime gameTime)
        {
            characterController.Update(gameTime);

            physics.Update(gameTime);

            // lightRenderer.AnimateLight(gameTime);

            base.Update(gameTime);
        }
    }
}

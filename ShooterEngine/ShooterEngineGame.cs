using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShooterEngine.Debug;

namespace ShooterEngine
{
    public class ShooterEngineGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private WorldRenderer worldRenderer;
        private SpriteFont helpFont;

        private KeyboardState previousKeyboardState;
        private FrameCounter frameCounter = new FrameCounter();

        public static bool DEBUG = false;

        public ShooterEngineGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;

            if (DEBUG)
            {
                graphics.SynchronizeWithVerticalRetrace = false;
                IsFixedTimeStep = false;
            }
            else
            {
                IsFixedTimeStep = true;
                TargetElapsedTime = System.TimeSpan.FromMilliseconds(16);
            }

            graphics.ToggleFullScreen();
        }

        protected override void Initialize()
        {
            worldRenderer = new WorldRenderer(this);
            Components.Add(worldRenderer);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            helpFont = Content.Load<SpriteFont>("Fonts\\HelpFont");
        }

        protected override void EndRun()
        {
            worldRenderer.EndRun();

            base.EndRun();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState ks = Keyboard.GetState();

            if (!ks.IsKeyDown(Keys.F1) && previousKeyboardState.IsKeyDown(Keys.F1))
            {
                worldRenderer.ToggleDebug();
            }

            previousKeyboardState = ks;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameCounter.Update(deltaTime);

            spriteBatch.Begin();
            spriteBatch.DrawString(helpFont, "FPS: " + frameCounter.AverageFramesPerSecond.ToString() + "\n\n" +
                "= CONTROLS =\n" +
                "Move:    W A S D\n" +
                "Look:     Mouse\n" +
                "Jump:    Space\n" +
                "Shoot:    Left Mouse Click\n" +
                "Debug:   F1\n" +
                "Exit:        Escape", new Vector2(10, 10), Color.White);
            spriteBatch.End();
        }
    }
}

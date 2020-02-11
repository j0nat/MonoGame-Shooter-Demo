using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShooterEngine
{
    class SkyDome : DrawableGameComponent
    {
        private Texture2D cloudMap;
        private Model skyDome;
        private BasicEffect effect;

        public SkyDome(Game game) : base(game)
        {

        }

        public override void Initialize()
        {
            effect = new BasicEffect(GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            skyDome = Game.Content.Load<Model>("Models\\dome");
            skyDome.Meshes[0].MeshParts[0].Effect = effect.Clone();
            cloudMap = Game.Content.Load<Texture2D>("Models\\cloudMap");

            base.LoadContent();
        }

        public void Draw(Camera camera)
        {
            Matrix[] modelTransforms = new Matrix[skyDome.Bones.Count];
            skyDome.CopyAbsoluteBoneTransformsTo(modelTransforms);

            Matrix wMatrix = Matrix.CreateTranslation(0, 0, 0) * Matrix.CreateScale(300);
            foreach (ModelMesh mesh in skyDome.Meshes)
            {
                foreach (BasicEffect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = modelTransforms[mesh.ParentBone.Index] * wMatrix;

                    currentEffect.TextureEnabled = true;
                    currentEffect.Texture = cloudMap;
                    currentEffect.World = worldMatrix;
                    currentEffect.Projection = camera.Projection;
                    currentEffect.View = camera.View;
                }
                mesh.Draw();
            }
        }
    }
}

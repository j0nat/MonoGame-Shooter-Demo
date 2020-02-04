using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShooterEngine
{
    class Camera
    {
        public Vector3 Position { get; set; }
        public Matrix Projection { get; protected set; }
        public Matrix View { get; set; }

        public Camera(GraphicsDevice graphicsDevice)
        {
            this.Position = Vector3.Zero;
            this.Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, graphicsDevice.Viewport.AspectRatio,
                0.05f, 10000.0f);
        }

        public Vector3 Right
        {
            get { return Matrix.Invert(View).Right; }
        }
    }
}

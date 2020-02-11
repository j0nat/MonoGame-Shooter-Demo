using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShooterEngine
{
    class Camera
    {
        public Matrix Projection { get; protected set; }
        public Matrix ShadowProjection { get; protected set; }
        public Matrix View { get; set; }
        public float NearZ { get; set; }
        public float FarZ { get; set; }

        public Camera(GraphicsDevice graphicsDevice)
        {
            this.Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, graphicsDevice.Viewport.AspectRatio,
                0.05f, 10000.0f);

            this.NearZ = 0.25f;
            this.FarZ = 250.0f;

            this.ShadowProjection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, graphicsDevice.Viewport.AspectRatio,
                0.25f, 250f);
        }

        public Vector3 Right
        {
            get { return Matrix.Invert(View).Right; }
        }

        public Vector3 Forward
        {
            get { return Matrix.Invert(View).Forward; }
        }

        public Matrix ViewProjection
        {
            get { return View * ShadowProjection; }
        }
    }
}

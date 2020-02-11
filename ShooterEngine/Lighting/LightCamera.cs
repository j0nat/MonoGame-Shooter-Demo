using Microsoft.Xna.Framework;

namespace ShooterEngine.Lighting
{
    public class LightCamera
    {
        private Matrix world, view, projection;
        private float minX, minY, maxX, maxY, nearZ, farZ;

        public LightCamera(float minX, float minY, float maxX, float maxY, float nearZ, float farZ)
        {
            this.nearZ = nearZ;
            this.farZ = farZ;
            this.minX = minX;
            this.minY = minY;
            this.maxX = maxX;
            this.maxY = maxY;

            this.world = Matrix.Identity;
            this.view = Matrix.Identity;

            CreateProjection();
        }

        public Matrix Projection
        {
            get { return projection; }
            set
            {
                projection = value;
            }
        }

        public Matrix ViewProjection
        {
            get { return view * projection; }
        }

        public float NearZ
        {
            get { return nearZ; }
            set
            {
                nearZ = value;
                CreateProjection();
            }
        }

        public float FarZ
        {
            get { return farZ; }
            set
            {
                farZ = value;
                CreateProjection();
            }
        }

        public void SetLookAt(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 up)
        {
            view = Matrix.CreateLookAt(cameraPosition, cameraTarget, up);
            world = Matrix.Invert(view);
            view = Matrix.Invert(world);
        }

        public void CreateProjection()
        {
            Projection = Matrix.CreateOrthographicOffCenter(minX, maxX, minY, maxY, NearZ, FarZ);
        }
    }
}
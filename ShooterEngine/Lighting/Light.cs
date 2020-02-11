using Microsoft.Xna.Framework;

namespace ShooterEngine.Lighting
{
    public class Light
    {
        public Vector3 Position { get; set; }
        public Color Color { get; set; }
        public float Intensity { get; set; }
        public float Radii { get; set; }

        public Light(Vector3 position, Color color, float intensity, float radii)
        {
            this.Position = position;
            this.Color = color;
            this.Intensity = intensity;
            this.Radii = radii;
        }
    }
}

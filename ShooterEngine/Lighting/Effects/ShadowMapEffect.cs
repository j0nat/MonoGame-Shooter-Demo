using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShooterEngine.Lighting.Effects
{
    public class ShadowMapEffect
    {
        private readonly Effect _innerEffect;
        private readonly EffectParameter _worldViewProjectionParameter;

        public Matrix WorldViewProjection { get; set; }
        public Texture2D Texture { get; set; }

        public ShadowMapEffect(GraphicsDevice graphicsDevice, Effect innerEffect)
        {
            _innerEffect = innerEffect;

            _worldViewProjectionParameter = _innerEffect.Parameters["WorldViewProjection"];
        }

        public void Apply()
        {
            _worldViewProjectionParameter.SetValue(WorldViewProjection);

            _innerEffect.CurrentTechnique.Passes[0].Apply();
        }
    }
}
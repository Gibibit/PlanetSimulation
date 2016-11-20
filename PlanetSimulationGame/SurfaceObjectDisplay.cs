using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlanetSimulation;

namespace PlanetSimulationGame
{
    public static class SurfaceObjectDisplay
    {
        public const float GAME_SCALE = PSGame.GAME_SCALE;

        public static void Draw(this SpriteBatch sb, Texture2D texture, Bush b)
            => sb.Draw(texture, b.Position.ToVector2() *GAME_SCALE, null,
                GetColor(b), 0f, Vector2.Zero, Vector2.One*GAME_SCALE, SpriteEffects.None, 0f);

        public static void Draw(this SpriteBatch sb, Texture2D texture, Pop p)
            => sb.Draw(texture, p.Position.ToVector2()*GAME_SCALE, null,
                GetColor(p), 0f, new Vector2(-0.075f), new Vector2(0.85f)*GAME_SCALE, SpriteEffects.None, 0f);

        public static Color GetColor(SurfaceObject so)
        {
            var pop = so as Pop;
            if (pop != null) return GetColor(pop);

            var bush = so as Bush;
            if(bush != null) return GetColor(bush);

            return Color.HotPink;
        }

        public static Color GetColor(Bush b) => Color.Lerp(Color.LightGreen, Color.DarkGreen, b.BerryFraction);

        public static Color GetColor(Pop p) => Color.Lerp(Color.Black, Color.LightGray, p.AgeFraction);
    }
}
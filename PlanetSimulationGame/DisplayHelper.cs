using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlanetSimulation;

namespace PlanetSimulationGame
{
    public static class DisplayHelper
    {
        public const float GAME_SCALE = PSGame.DRAW_SCALE;

        public static void Draw(this SpriteBatch sb, Texture2D pixel, Bush b)
        {
            Color color;
            Vector2 scale, origin;
            if (b.Grown)
            {
                color = GetColor(b);
                scale = Vector2.One*GAME_SCALE;
                origin = Vector2.Zero;
            }
            else
            {
                color = Color.Red;
                scale = new Vector2(0.8f*GAME_SCALE);
                origin = new Vector2(-0.1f);
            }
            sb.Draw(pixel, b.Position.ToVector2()*GAME_SCALE, null,
                color, 0f, origin, scale, SpriteEffects.None, 0f);
        }

        public static void Draw(this SpriteBatch sb, Texture2D pixel, Pop p)
        {
            sb.Draw(pixel, p.Position.ToVector2()*GAME_SCALE, null,
                GetColor(p), 0f, new Vector2(-0.125f), new Vector2(0.75f)*GAME_SCALE, SpriteEffects.None, 0f);

            sb.Draw(pixel, p.Position.ToVector2()*GAME_SCALE, null,
                Color.White, 0f, new Vector2(-0.125f), 
                new Vector2(0.75f*GAME_SCALE*p.FullnessFraction, 3f), SpriteEffects.None, 0f);
        }

        public static Color GetColor(SurfaceObject so)
        {
            var pop = so as Pop;
            if (pop != null) return GetColor(pop);

            var bush = so as Bush;
            if(bush != null) return GetColor(bush);

            return Color.HotPink;
        }

        public static Color GetFertilityColor(int fertility)
            => Color.Lerp(Color.SandyBrown, Color.SaddleBrown, fertility/100f);

        public static Color GetColor(Bush b) 
            => Color.Lerp(Color.LightGreen, Color.DarkGreen, b.BerryFraction);

        public static Color GetColor(Pop p) 
            => Color.Lerp(Color.Black, Color.LightGray, p.AgeFraction);
    }
}
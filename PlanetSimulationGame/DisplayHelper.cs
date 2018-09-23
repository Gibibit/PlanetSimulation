using LineBatch;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PlanetSimulation;

namespace PlanetSimulationGame
{
    public static class DisplayHelper
    {
        public static bool DebugDraw;

        public static void Init(ContentManager content, Texture2D pixel)
        {
            _heart = content.Load<Texture2D>("Heart");
            _font = content.Load<SpriteFont>("DebugFont");
            _pixel = pixel;
        }

        private static Texture2D _pixel;
        private static Texture2D _heart;
        private static SpriteFont _font;

        public const float GAME_SCALE = PSGame.DRAW_SCALE;

        public static void Draw(this SpriteBatch sb, Bush b)
        {
            Color color;
            float scale;
            Vector2 offset;
            if (b.Grown)
            {
                color = GetColor(b);
                scale = 1f;
                offset = Vector2.Zero;
            }
            else
            {
                color = Color.Red;
                scale = 0.25f;
                offset = new Vector2((1f - scale)/2f);
            }
            var drawPos = (b.Position.ToVector2() + offset) * GAME_SCALE;
            var drawScale = scale*GAME_SCALE;
            sb.Draw(_pixel, drawPos, null, color, 0f, Vector2.Zero, drawScale, SpriteEffects.None, 0f);
        }

        public static void Draw(this SpriteBatch sb, Pop p)
        {
			var popSize = 0.75f*GAME_SCALE;

			sb.Draw(_pixel, p.Position.ToVector2()*GAME_SCALE, null,
                GetColor(p), 0f, new Vector2(-0.125f), popSize, SpriteEffects.None, 0f);

            sb.Draw(_pixel, p.Position.ToVector2()*GAME_SCALE, null,
                Color.White, 0f, new Vector2(-0.125f), 
                new Vector2(popSize*p.FullnessFraction, 3f), SpriteEffects.None, 0f);

            if(p.DesiresBreed) {
                sb.Draw(_heart, (p.Position.ToVector2() + new Vector2(0.5f, 0.5f))*GAME_SCALE, 
					null, Color.White, 0f, new Vector2(0f, _heart.Bounds.Height), popSize/2f/_heart.Bounds.Width, SpriteEffects.None, 0f);
            }

            if(DebugDraw) {
                if (p.LastTargetBush != null) {
                    sb.DrawLine(p.Position.ToVector2()*GAME_SCALE + Vector2.One*GAME_SCALE*0.5f, 
                        p.LastTargetBush.Position.ToVector2()*GAME_SCALE + Vector2.One*GAME_SCALE*0.5f, Color.White);
                }
                if(p.IsWandering) {
                    var center = p.Position.ToVector2()*GAME_SCALE + Vector2.One*GAME_SCALE*0.5f;
                    sb.DrawLine(center, center + p.WanderDirection.ToVector2()*GAME_SCALE*0.5f, Color.White);
                }

                if(p.DesiresForage) sb.DrawString(_font, "F", p.Position.ToVector2()*GAME_SCALE, Color.White);
                sb.DrawString(_font, p.GetFood(FoodTypes.Berry).ToString(), (p.Position.ToVector2() + new Vector2(0.6f))*GAME_SCALE, Color.White);
            }
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
            => Color.Lerp(p.Gender == Genders.Female ? Color.Pink : Color.LightBlue, Color.DarkGray, p.AgeFraction);
    }
}
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PlanetSimulation;

namespace PlanetSimulationGame
{
    public class PSGame : Game
    {
        private int _stepDelay = 33;

        public int StepDelay
        {
            get
            {
                return _stepDelay;
            }
            set
            {
                _stepDelay = MathHelper.Clamp(value, 1, 500);
                TargetElapsedTime = new TimeSpan(0, 0, 0, 0, _stepDelay);
            }
        }

        public const float DRAW_SCALE = 30f;
        public const int STEP_AMOUNT = 1;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Planet _planet;
        private Texture2D _pixel;
        private SpriteFont _debugFont;

        public PSGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, _stepDelay);
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _pixel = new Texture2D(_graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _pixel.SetData(new[] { Color.White });

            _planet = new Planet(PlanetSimulationConfig.DefaultConfig);
            _planet.InitRandom();

            _debugFont = Content.Load<SpriteFont>("DebugFont");

            _graphics.PreferredBackBufferWidth = (int) (_planet.Config.Width *DRAW_SCALE);
            _graphics.PreferredBackBufferHeight = (int) (_planet.Config.Height *DRAW_SCALE) + (int) _debugFont.MeasureString("X").Y*4;
            _graphics.ApplyChanges();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            for (int i = 0; i < STEP_AMOUNT; i++)
            {
                _planet.Step();
            }

            var kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.OemPlus)) StepDelay += StepDelay/10 + 1;
            if (kb.IsKeyDown(Keys.OemMinus)) StepDelay -= StepDelay/10 + 1;
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice.Clear(Color.SaddleBrown);

            _spriteBatch.Begin();

            for (int x = 0; x < _planet.Config.Width; ++x)
                for (int y = 0; y < _planet.Config.Height; ++y)
                {
                    var color = DisplayHelper.GetFertilityColor(_planet.GetFertility(x, y));
                    _spriteBatch.Draw(_pixel, new Vector2(x, y)*DRAW_SCALE, null, color, 
                        0f, Vector2.Zero, Vector2.One*DRAW_SCALE, SpriteEffects.None, 0f);
                }

            _planet.Bushes.ForEach(b => _spriteBatch.Draw(_pixel, b));
            _planet.Population.ForEach(p => _spriteBatch.Draw(_pixel, p));
            _spriteBatch.DrawString(_debugFont, 
                $"Step {_planet.CurrentStep} | DT {StepDelay}\n" +
                $"Pops {_planet.Population.Count} | PAAF {Math.Round(_planet.AvgPopAgeFraction, 2)}" +
                $" | Generation {_planet.MaxGeneration} | Deaths ({_planet.AgeDeaths}/{_planet.StarvationDeaths})\n" +
                $"Bushes {_planet.Bushes.Count} | BAAF {Math.Round(_planet.AvgBushAgeFraction, 2)}" +
                $" | Fertility {_planet.TotalFertility} | Digestions {_planet.Population.Sum(p => p.DigestionAmount)}", 
                new Vector2(0f, _planet.Config.Height *DRAW_SCALE), Color.DarkGray);
            _spriteBatch.End();
        }
    }
}

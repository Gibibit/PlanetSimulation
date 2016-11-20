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
        private int _stepDelay = 100;

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

        public const float GAME_SCALE = 15f;
        public const int STEP_AMOUNT = 1;
        public const int POP_AMOUNT = 3;
        public const int BUSH_AMOUNT = 10;
        public const int PLANET_WIDTH = 75;
        public const int PLANET_HEIGHT = 50;

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

            _planet = new Planet(PLANET_WIDTH, PLANET_HEIGHT, PlanetSimulationConfig.DefaultConfig);
            _planet.InitRandom(POP_AMOUNT, BUSH_AMOUNT);

            _debugFont = Content.Load<SpriteFont>("DebugFont");

            _graphics.PreferredBackBufferWidth = (int) (_planet.Width*GAME_SCALE);
            _graphics.PreferredBackBufferHeight = (int) (_planet.Height*GAME_SCALE) + (int) _debugFont.MeasureString("X").Y;
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
            _planet.Bushes.ForEach(b => _spriteBatch.Draw(_pixel, b));
            _planet.Population.ForEach(p => _spriteBatch.Draw(_pixel, p));
            _spriteBatch.DrawString(_debugFont, 
                $"Step {_planet.CurrentStep} | DT {StepDelay} | Pops {_planet.Population.Count}" +
                $" | PAAF {Math.Round(_planet.Population.Average(p => p.AgeFraction), 2)} | Generation {_planet.MaxGeneration}" +
                $" | Bushes {_planet.Bushes.Count} | BAAF {Math.Round(_planet.Bushes.Average(b => b.AgeFraction), 2)}", 
                new Vector2(0f, _planet.Height*GAME_SCALE), Color.DarkGray);
            _spriteBatch.End();
        }
    }
}

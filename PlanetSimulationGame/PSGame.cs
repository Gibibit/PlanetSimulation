using System;
using System.Collections.Generic;
using System.Linq;
using LineBatch;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PlanetSimulation;

namespace PlanetSimulationGame
{
    public class PSGame : Game
    {
        private KeyboardState _prevKb;
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
		private bool _firstStep = true;
		private bool _paused = true;

        public PSGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, _stepDelay);
            IsMouseVisible = true;
            _prevKb = Keyboard.GetState();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _pixel = new Texture2D(_graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _pixel.SetData(new[] { Color.White });
            DisplayHelper.Init(Content, _pixel);

            _planet = new Planet(PlanetSimulationConfig.DefaultConfig);
            _planet.Init();

            _debugFont = Content.Load<SpriteFont>("DebugFont");

			_graphics.PreferredBackBufferWidth = (int)(_planet.Config.Width*DRAW_SCALE);
            _graphics.PreferredBackBufferHeight = (int) (_planet.Config.Height*DRAW_SCALE) + (int) _debugFont.MeasureString("X").Y*4;
			_graphics.ApplyChanges();

			SpriteBatchExtensions.GraphicsDevice = GraphicsDevice;
        }

        protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if(_firstStep)
			{
				_firstStep = false;
				CenterWindow();
			}

			if(!_paused)
			{
				for(int i = 0; i < STEP_AMOUNT; i++)
				{
					_planet.Step();
				}
			}

            var kb = Keyboard.GetState();

            if (kb.IsKeyDown(Keys.OemPlus) && (StepDelay > 10 || _prevKb.IsKeyUp(Keys.OemPlus))) StepDelay += StepDelay/10 + 1;
            if (kb.IsKeyDown(Keys.OemMinus) && (StepDelay > 10 || _prevKb.IsKeyUp(Keys.OemMinus))) StepDelay -= StepDelay/10 + 1;
			if(kb.IsKeyDown(Keys.D) && _prevKb.IsKeyUp(Keys.D)) DisplayHelper.DebugDraw = !DisplayHelper.DebugDraw;
			if(kb.IsKeyDown(Keys.Space) && _prevKb.IsKeyUp(Keys.Space)) _paused = !_paused;

			_prevKb = kb;
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice.Clear(Color.SaddleBrown);

            _spriteBatch.Begin();

            for (int x = 0; x < _planet.Config.Width; ++x)
                for (int y = 0; y < _planet.Config.Height; ++y)
                {
                    var color = _planet.GetFertilityColor(_planet.GetFertility(x, y));
                    _spriteBatch.Draw(_pixel, new Vector2(x, y)*DRAW_SCALE, null, color, 
                        0f, Vector2.Zero, Vector2.One*DRAW_SCALE, SpriteEffects.None, 0f);
                }

            _planet.Bushes.ForEach(b => _spriteBatch.Draw(b));
            _planet.Population.ForEach(p => _spriteBatch.Draw(p));
            _spriteBatch.DrawString(_debugFont, 
                $"Step {_planet.CurrentStep} | DT {StepDelay}\n" +
                $"Pops {_planet.Population.Count} | PAAF {Math.Round(_planet.AvgPopAgeFraction, 2)}" +
                $" | Generation {_planet.MaxGeneration} | Deaths ({_planet.AgeDeaths}/{_planet.StarvationDeaths})\n" +
                $"Bushes {_planet.Bushes.Count} | BAAF {Math.Round(_planet.AvgBushAgeFraction, 2)} | Color: {_planet.GetFertilityColor(_planet.MaxFertility)}" +
                $" | Fertility {_planet.TotalFertility} | MaxFertility {_planet.MaxFertility} | CurrentMaxFertility {_planet.CurrentMaxFertility}",
				new Vector2(0f, _planet.Config.Height *DRAW_SCALE), Color.DarkGray);
            _spriteBatch.End();
		}

		private void CenterWindow()
		{
			var halfScreenSize = new Point(GraphicsDevice.Adapter.CurrentDisplayMode.Width/2, GraphicsDevice.Adapter.CurrentDisplayMode.Height/2);
			Window.Position = halfScreenSize - new Point(_graphics.PreferredBackBufferWidth/2, _graphics.PreferredBackBufferHeight/2);
		}
	}
}

﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PlanetSimulation;

namespace PlanetSimulationGame
{
    public static class ExtensionsMethods
    {
        public const float GAME_SCALE = PSGame.GAME_SCALE;

        public static void Draw(this SpriteBatch sb, Texture2D texture, Bush b)
            => sb.Draw(texture, b.Position.ToVector2()*GAME_SCALE, null, Color.Green, 0f, Vector2.Zero, Vector2.One*GAME_SCALE, SpriteEffects.None, 0f);

        public static void Draw(this SpriteBatch sb, Texture2D texture, Pop p)
            => sb.Draw(texture, p.Position.ToVector2()*GAME_SCALE, null, Color.Black, 0f, Vector2.Zero, Vector2.One*GAME_SCALE, SpriteEffects.None, 0f);
    }

    public class PSGame : Game
    {
        public const float GAME_SCALE = 10f;
        public const double STEP_SPEED = 0d;
        private const int POP_AMOUNT = 5;
        private const int BUSH_AMOUNT = 1500;
        public const int PLANET_WIDTH = 50;
        public const int PLANET_HEIGHT = 50;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Planet _planet;
        private Texture2D _pixel;
        private TimeSpan _lastStep;

        public PSGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _lastStep = new TimeSpan(0);
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _pixel = new Texture2D(_graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _pixel.SetData(new[] { Color.White });

            _planet = new Planet(PLANET_WIDTH, PLANET_HEIGHT);
            _planet.InitRandom(POP_AMOUNT, BUSH_AMOUNT);

            _graphics.PreferredBackBufferWidth = (int) (_planet.Width*GAME_SCALE);
            _graphics.PreferredBackBufferHeight = (int) (_planet.Height*GAME_SCALE);
            _graphics.ApplyChanges();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            if(gameTime.TotalGameTime.TotalSeconds - _lastStep.TotalSeconds >= STEP_SPEED) {
                _planet.Step();
                _lastStep = new TimeSpan(gameTime.TotalGameTime.Ticks);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice.Clear(Color.SaddleBrown);

            _spriteBatch.Begin();
            _planet.Bushes.ForEach(b => _spriteBatch.Draw(_pixel, b));
            _planet.Population.ForEach(p => _spriteBatch.Draw(_pixel, p));
            _spriteBatch.End();
        }
    }
}
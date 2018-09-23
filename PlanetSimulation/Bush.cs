using System;
using System.Collections.Generic;
using System.Linq;
using HermansGameDev.RandomExtensions;
using Microsoft.Xna.Framework;

namespace PlanetSimulation
{
    public class Bush : SurfaceObject
    {
        public int Age;
        public bool Grown => Age > Planet.Config.BushGrowDelay;
        public float AgeFraction => (float)Age / Planet.Config.BushMaxAge;

        public int Berries;
        public float BerryFraction => (float) Berries/Planet.Config.BushMaxBerries;
        public float BerryGrowChance => Planet.Config.BushBerryGrowChance*(Planet.Config.BushMaxAge - Age)/Planet.Config.BushMaxAge;


        private Random _random;

        public Bush(Planet planet, Point position) : base(planet, position)
        {
            _random = planet.Random;
            Berries = Planet.Config.BushStartingBerries;
        }

        public void Step()
        {
            Age++;

			if(Age <= Planet.Config.BushMaxGrowAge && Grown && _random.NextFloat() < Planet.Config.BushBerryGrowChance)
			{
				var fertileTiles = Planet.GetCross(Position)
				.Where(p => Planet.IsInBounds(p) && Planet.GetFertility(p) >= Planet.Config.BushBerryGrowCost).ToList();

				if(fertileTiles.Count > 0 && Planet.SpendFertility(_random.Choose<Point>(fertileTiles), Planet.Config.BushBerryGrowCost))
				{
					Berries++;
				}
				if(Berries > Planet.Config.BushMaxBerries)
				{
					Berries = Planet.Config.BushMaxBerries;
					Planet.TrySeedBushCross(Position);
				}
			}
        }
    }
}
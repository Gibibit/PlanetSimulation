using System;
using HermansGameDev.RandomExtensions;
using Microsoft.Xna.Framework;

namespace PlanetSimulation
{
    public class Bush : SurfaceObject
    {
        public int Berries;

        public int Age;

        public float BerryFraction => (float) Berries/Planet.Config.BushMaxBerries;

        public float GrowChance => Planet.Config.BushBerryGrowChance*(Planet.Config.BushMaxAge - Age)/Planet.Config.BushMaxAge;

        public float AgeFraction => (float)Age/Planet.Config.BushMaxAge;

        private Random _random;

        public Bush(Planet planet, Point position) : base(planet, position)
        {
            _random = planet.Random;
            Berries = Planet.Config.BushStartingBerries;
        }

        public void Step()
        {
            Age++;
            if (Age <= Planet.Config.BushMaxGrowAge && _random.NextFloat() < Planet.Config.BushBerryGrowChance) Berries++;
            if (Berries > Planet.Config.BushMaxBerries) Berries = Planet.Config.BushMaxBerries;
        }
    }
}
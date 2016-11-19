using System;
using HermansGameDev.RandomExtensions;
using Microsoft.Xna.Framework;

namespace PlanetSimulation
{
    public class Bush : SurfaceObject
    {
        public int Berries;
        
        public float BerryFraction => (float) Berries/Planet.Config.BushMaxBerries;

        private Random _random;

        public Bush(Planet planet, Point position) : base(planet, position)
        {
            _random = planet.Random;
            Berries = Planet.Config.BushStartingBerries;
        }

        public void Step()
        {
            if (_random.NextFloat() < Planet.Config.BushBerryGrowChance) Berries++;
            if (Berries > Planet.Config.BushMaxBerries) Berries = Planet.Config.BushMaxBerries;
        }
    }
}
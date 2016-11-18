using System;
using System.Linq;
using HermansGameDev.RandomExtensions;
using Microsoft.Xna.Framework;

namespace PlanetSimulation
{
    public class Pop : SurfaceObject
    {
        public int Fullness;
        public int Age;
        public readonly int Generation;

        public float AgeFraction => (float)Age/Planet.PopMaxAge;

        private readonly Random _random;

        public Pop(Planet planet, Point position, int generation = 0) : base(planet, position)
        {
            _random = planet.Random;
            Generation = generation;
            Fullness = Planet.PopStartingFullness;
        }

        public void Step()
        {
            Age++;
            Fullness--;

            var bush = Planet.Bushes.Find(b => b.Position == Position);
            if (bush != null)
            {
                var berries = Math.Min(bush.Berries, Planet.PopBerryConsumption);
                Fullness += berries;
                bush.Berries -= berries;
                if(bush.Berries <= 0) Planet.Bushes.Remove(bush);
            }
            else
            {
                // Find bushes within foraging range
                var bushes = Planet.Bushes.FindAll(b => b.Position.Manhattan(Position) <= Planet.PopForagingRange);
                if (bushes.Count > 0)
                {
                    // Find closest bush
                    var closest = bushes.Min(b => b.Position.Manhattan(Position));
                    // Remove farther away bushes
                    bushes.RemoveAll(b => b.Position.Manhattan(Position) > closest);
                    // Choose randomly from remaining bushes
                    var target = _random.Choose<Bush>(bushes);
                    // Navigate towards bush
                    var dir = target.Position - Position;
                    var movement = Point.Zero;
                    if (dir.X != 0 && dir.Y != 0) movement = _random.Choose(dir.XDir(), dir.YDir());
                    else if (dir.X != 0) movement = dir.XDir();
                    else if (dir.Y != 0) movement = dir.YDir();

                    Position += movement;
                }
            }

            if (Fullness >= Planet.PopBreedingFullness && Age > Planet.PopBreedingAge)
            {
                Fullness -= Planet.PopBreedingCost;
                Planet.AddPop(Position, Generation + 1);
            }
        }
    }
}
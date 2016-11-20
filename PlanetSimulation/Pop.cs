using System;
using System.Collections.Generic;
using System.Linq;
using HermansGameDev.RandomExtensions;
using Microsoft.Xna.Framework;

namespace PlanetSimulation
{
    public class Pop : SurfaceObject
    {
        private Food[] _food;
        public int Fullness;
        public int Age;
        public readonly int Generation;

        private int _nextWalk;

        public float AgeFraction => (float)Age/Planet.Config.PopMaxAge;

        private readonly Random _random;

        public Pop(Planet planet, Point position, int generation = 0) : base(planet, position)
        {
            _random = planet.Random;
            Generation = generation;
            Fullness = Planet.Config.PopStartingFullness;
            _nextWalk = Planet.Random.Next(Planet.Config.PopWalkDelay);
            _food = new Food[Enum.GetValues(typeof(FoodTypes)).Length];
            for (int i = 0; i < _food.Length; i++)
            {
                _food[i] = new Food((FoodTypes) i, 0);
            }
        }

        public int GetFood(FoodTypes type) => _food[(int) type].Amount;

        public void AddFood(FoodTypes type, int amount) => _food[(int) type].Amount += amount;

        public void EatFood(FoodTypes type, int amount)
        {
            var food = _food[(int) type];
            var actualAmount = Math.Min(amount, food.Amount);
            Fullness += actualAmount*FoodType.Power(type);
            _food[(int) type].Amount -= actualAmount;
        }

        public void Step()
        {
            Age++;
            Fullness--;

            var bush = Planet.Bushes.Find(b => b.Position == Position && b.Berries > 0);
            if (bush != null)
            {
                var berries = Math.Min(bush.Berries, Planet.Config.PopBerryHarvestAmount);
                AddFood(FoodTypes.Berry, berries);
                bush.Berries -= berries;
                bush.Age += berries*2;
                //if(bush.Berries <= 0) Planet.Bushes.Remove(bush);
            }
            else
            {
                _nextWalk--;

                if (_nextWalk <= 0)
                {
                    _nextWalk = Planet.Config.PopWalkDelay;

                    // Find bushes within foraging range
                    var bushes = Planet.Bushes.FindAll(b => b.Position.Manhattan(Position) <= Planet.Config.PopVisionRange && b.Berries > 0);
                    if (bushes.Count > 0)
                    {
                        // Find best bush heuristic
                        var heuristic = new Func<Bush, int>(
                            b => {
                                var hasOtherPop = Planet.Population.Any(p => p != this && p.Position == Position);
                                var distance = b.Position.Manhattan(Position);
                                return Planet.Config.PopVisionRange
                                       - distance
                                       + b.Berries
                                       - (hasOtherPop ? distance*Planet.Config.PopWalkDelay : 0);
                            });
                        var fitness = bushes.Max(heuristic);
                        // Remove bushes with less than optimal heuristic, with a small error margin
                        bushes.RemoveAll(b => heuristic(b) < fitness - 2);
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
            }

            if (Fullness < Planet.Config.PopFullnessHungry)
            {
                EatFood(FoodTypes.Berry, 10);
            }

            if (Fullness >= Planet.Config.PopBreedingFullness && Age > Planet.Config.PopBreedingAge)
            {
                Fullness -= Planet.Config.PopBreedingCost;
                Planet.AddPop(Position, Generation + 1);
            }
        }
    }
}
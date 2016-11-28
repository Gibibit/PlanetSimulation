using System;
using System.Collections.Generic;
using System.Linq;
using HermansGameDev.RandomExtensions;
using Microsoft.Xna.Framework;

namespace PlanetSimulation
{
    public class Pop : SurfaceObject
    {
        private List<Point> Directions => new List<Point>
        {
            new Point(1, 0),
            new Point(0, 1),
            new Point(-1, 0),
            new Point(0, -1)
        };

        private Food[] _food;
        private List<int> _digestionTimers;
        private Point _wanderDirection;

        public int Fullness;
        public int Age;
        public readonly int Generation;
        public int DigestionAmount => _digestionTimers.Count;

        public float FullnessFraction => MathHelper.Clamp((float)Fullness/Planet.Config.PopBreedingFullness, 0f, 1f);

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
            _digestionTimers = new List<int>();
            _wanderDirection = _random.Choose<Point>(Directions);
        }

        public int GetFood(FoodTypes type) => _food[(int) type].Amount;

        public void AddFood(FoodTypes type, int amount) => _food[(int) type].Amount += amount;

        public void EatFood(FoodTypes type, int amount)
        {
            var food = _food[(int) type];
            var actualAmount = Math.Min(amount, food.Amount);
            Fullness += actualAmount*FoodType.Power(type);
            _food[(int) type].Amount -= actualAmount;
            for(int i = 0; i < actualAmount; ++i)
                _digestionTimers.Add(_random.Next(Planet.Config.PopBerryDigestDelayMin, Planet.Config.PopBerryDigestDelayMax + 1));
        }

        public void Step()
        {
            Age++;
            Fullness -= Planet.Config.PopFullnessConsumption;

            var bush = Planet.Bushes.Find(b => b.Position == Position && b.Berries > 0);
            if (bush != null)
            {
                var berries = Math.Min(bush.Berries, Planet.Config.PopBerryHarvestAmount);
                AddFood(FoodTypes.Berry, berries);
                bush.Berries -= berries;
                bush.Age += berries*2;
            }
            else
            {
                _nextWalk--;

                if (_nextWalk <= 0)
                {
                    _nextWalk = Planet.Config.PopWalkDelay;

                    // Find bushes within foraging range
                    var bushes = Planet.Bushes.FindAll(
                        b => b.Position.Manhattan(Position) <= Planet.Config.PopVisionRange && b.Berries > 0 && b.Grown
                    );
                    var wander = false;
                    if (bushes.Count > 0)
                    {
                        // Find best bush heuristic
                        var heuristic = new Func<Bush, int>(
                            b =>
                            {
                                var hasOtherPop = Planet.Population.Any(p => p != this && p.Position == Position);
                                var distance = b.Position.Manhattan(Position);
                                return Planet.Config.PopVisionRange
                                       - distance
                                       + b.Berries
                                       - (hasOtherPop ? distance*Planet.Config.PopWalkDelay : 0);
                            });
                        var fitness = bushes.Max(heuristic);
                        if (fitness > Planet.Config.PopWanderFitnessThreshold)
                        {
                            // Remove bushes with less than optimal heuristic, with a small error margin
                            bushes.RemoveAll(b => heuristic(b) < fitness - Planet.Config.PopFoodHeuristicRandom);
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
                        else
                        {
                            wander = true;
                        }
                        

                    }
                    else
                    {
                        wander = true;
                    }

                    if (wander)
                    {
                        // Wander
                        if (_random.NextDouble() < Planet.Config.PopWanderTurnChance 
                            || !Planet.IsInBounds(Position + _wanderDirection))
                            _wanderDirection = _random.Choose<Point>(Directions.Where(d => Planet.IsInBounds(Position + d)).ToList());
                        Position += _wanderDirection;
                    }
                }
            }

            if (Fullness < Planet.Config.PopFullnessHungry)
            {
                EatFood(FoodTypes.Berry, Planet.Config.PopEatAmount);
            }

            if (Fullness >= Planet.Config.PopBreedingFullness && Age > Planet.Config.PopBreedingAge)
            {
                Fullness -= Planet.Config.PopBreedingCost;
                Planet.AddPop(Position, Generation + 1);
            }

            // Digest food
            _digestionTimers = _digestionTimers.Select(d => d - 1).ToList();
            _digestionTimers.Where(d => d == 0).ToList().ForEach(d =>
            {
                Planet.TrySeedBushCross(Position);
                Planet.AddFertility(Position, Planet.Config.PopPoopFertility);
            });
            _digestionTimers.RemoveAll(d => d <= 0);
        }
    }
}
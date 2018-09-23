using System;
using System.Collections.Generic;
using System.Linq;
using HermansGameDev.RandomExtensions;
using Microsoft.Xna.Framework;

namespace PlanetSimulation
{
    public enum Genders
    {
        Male,
        Female
    }

    public class Pop : SurfaceObject
    {
        #region Movement

        private List<Point> Directions => new List<Point>
        {
            new Point(1, 0),
            new Point(0, 1),
            new Point(-1, 0),
            new Point(0, -1)
        };
        private Point _wanderDirection;
        private int _nextWalk;
        private bool PositionFree(Point target) => Planet.IsInBounds(target) && Planet.Population.All(p => p.Position != target) && Planet.Bushes.All(b => b.Position != target);
        private bool _wander;
        public bool IsWandering => _wander;
        public Point WanderDirection => _wanderDirection;

        #endregion Movement


        #region Food

        private Food[] _food;
        private List<int> _digestionTimers;
        public int Fullness;
        public int DigestionAmount => _digestionTimers.Count;
        public bool IsHungry => Fullness <= Planet.Config.PopFullnessHungry;
        public float FullnessFraction => MathHelper.Clamp((float)Fullness/Planet.Config.PopBreedingFullness, 0f, 1f);
        public bool DesiresForage => _foraging || GetFood(FoodTypes.Berry) < Planet.Config.PopFoodStockpileMin;
        private bool _foraging;
        public Bush LastTargetBush;

        #endregion Food


        #region Breeding

        public bool CanBreed => _desiresBreed 
            && LastBreedStep + Planet.Config.PopBreedingDelay <= Planet.CurrentStep 
            && HasBreedingFullness && HasBreedingAge;

        public bool DesiresBreed => _desiresBreed && Planet.Population.Count < Planet.Config.PopLimit;

        public int LastBreedStep = int.MinValue;
        private bool _desiresBreed;
        public bool HasBreedingFullness => Fullness >= Planet.Config.PopBreedingFullness;
        public bool HasBreedingAge => Age >= Planet.Config.PopBreedingAge;

        #endregion Breeding


        #region General stats

        public Genders Gender;
        public int Age;
        public readonly int Generation;
        public float AgeFraction => (float)Age/Planet.Config.PopMaxAge;

        #endregion General stats


        private readonly Random _random;
        private Pop _targetMate;

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
            Gender = _random.Choose(Genders.Male, Genders.Female);
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

            var foraged = false;

            _foraging = DesiresForage;
            if (GetFood(FoodTypes.Berry) >= Planet.Config.PopFoodStockpileMax) _foraging = false;
            if (_foraging) foraged = Forage();

            if(!foraged)
            {
                _nextWalk--;

                if (_nextWalk <= 0)
                {
                    _nextWalk = Planet.Config.PopWalkDelay;
                    _wander = !_foraging || !SelectAndMoveToBush();
                    if (DesiresBreed && _targetMate != null) MoveTowards(_targetMate.Position);
                    else if (_wander) Wander();
                }
            }

            if (IsHungry) EatFood(FoodTypes.Berry, Planet.Config.PopEatAmount);

            Breed();

            DigestFood();
        }

        private void MoveTowards(Point target)
        {
            var dir = target - Position;
            var movement = Point.Zero;
            if (dir.X != 0 && dir.Y != 0) movement = _random.Choose(dir.XDir(), dir.YDir());
            else if (dir.X != 0) movement = dir.XDir();
            else if (dir.Y != 0) movement = dir.YDir();
            if (PositionFree(Position + movement)) Position += movement;
            else {
                // Try another random direction
                movement = _random.Choose<Point>(Directions.Where(d => d != movement).ToList());
                if (PositionFree(Position + movement)) Position += movement;
            }
        }

        private bool Forage()
        {
            var bushesInRange = Planet.Bushes.FindAll(b => Position.Manhattan(b.Position) <= Planet.Config.PopHarvestRange);
            if (bushesInRange.Count <= 0) return false; // No bushes in range

            var nearest = bushesInRange.Min(b => Position.Manhattan(b.Position));
            var nearestBushes = bushesInRange.FindAll(b => Position.Manhattan(b.Position) == nearest);
            var maxBerries = nearestBushes.Max(b => b.Berries);
            var bush = _random.Choose<Bush>(nearestBushes.FindAll(b => b.Berries == maxBerries));

            if (bush == null || bush.Berries <= 0) return false; // No bush with berries found

            var berries = Math.Min(bush.Berries, Planet.Config.PopBerryHarvestAmount);
            AddFood(FoodTypes.Berry, berries);
            bush.Berries -= berries;
            bush.Age += berries*2;
            return true;
        }

        private bool SelectAndMoveToBush()
        {
            // Find bushes within foraging range
            var bushes = Planet.BushesInRange[Position.X, Position.Y].FindAll(bd => bd.Bush.Berries > 0 && bd.Bush.Grown);

            if (bushes.Count == 0) return false; // No bushes in sight
            
            // Find best bush heuristic
            var heuristic = new Func<BushDistance, int>(
                bd =>
                {
                    var hasOtherPop = Planet.Population.Any(p => p != this && p.Position == Position);
                    var distance = bd.Distance;
                    return Planet.Config.PopVisionRange
                            - distance
                            + bd.Bush.Berries
                            - (hasOtherPop ? distance*Planet.Config.PopWalkDelay : 0);
                });

            var fitness = bushes.Max(heuristic);
            if (fitness <= Planet.Config.PopWanderFitnessThreshold) return false; // Did not find bush with high enough threshold
                
            // Remove bushes with less than optimal heuristic, with a small error margin
            bushes.RemoveAll(b => heuristic(b) < fitness - Planet.Config.PopFoodHeuristicRandom);
            // Choose randomly from remaining bushes
            var target = _random.Choose<BushDistance>(bushes).Bush;
            LastTargetBush = target;
            // Navigate towards bush
            MoveTowards(target.Position);

            return true; // Succesfully moved toward target bush
        }

        private void Wander()
        {
            LastTargetBush = null;
            if (_random.NextDouble() < Planet.Config.PopWanderTurnChance
                || !Planet.IsInBounds(Position + _wanderDirection)
                || !PositionFree(Position + _wanderDirection)) {
                var directions = Directions.Where(d => PositionFree(Position + d)).ToList();
                if (directions.Any()) _wanderDirection = _random.Choose<Point>(directions);
            }
            MoveTowards(Position + _wanderDirection);
        }

        /// <summary>
        /// Procs breeding chance and adds new pops
        /// <para>O(number of pops)</para>
        /// </summary>
        private void Breed()
        {
            if (HasBreedingAge && !_desiresBreed && _random.NextDouble() < Planet.Config.PopBreedingDesireChance) _desiresBreed = true;
            if (CanBreed) {
                _targetMate = Planet.Population.Find(p => p != this && p.CanBreed && Gender != p.Gender);

                if (_targetMate != null && _targetMate.Position.Manhattan(Position) <= Planet.Config.PopBreedingRange) {
                    Fullness -= Planet.Config.PopBreedingCost;
                    _targetMate.Fullness -= Planet.Config.PopBreedingCost;
                    Planet.AddPop(_random.Choose(Position, _targetMate.Position), Generation + 1);
                    LastBreedStep = Planet.CurrentStep;
                    _desiresBreed = false;
                    _targetMate = null;
                }
            }
        }

        /// <summary>
        /// Processes food digestion timers and poop fertility
        /// <para>O(num food types)</para>
        /// </summary>
        private void DigestFood()
        {
            _digestionTimers = _digestionTimers.Select(d => d - 1).ToList();
            _digestionTimers.Where(d => d == 0).ToList().ForEach(d =>
			{
				Planet.AddFertility(Position, Planet.Config.PopPoopFertility);
				Planet.TrySeedBushCross(Position);
            });
            _digestionTimers.RemoveAll(d => d <= 0);
        }
    }
}
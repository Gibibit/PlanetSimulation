﻿using System;
using System.Collections.Generic;
using System.Linq;
using HermansGameDev.RandomExtensions;
using Microsoft.Xna.Framework;

namespace PlanetSimulation
{
    public static class ExtensionMethods
    {
        public static int Manhattan(this Point p, Point to) => Math.Abs(p.X - to.X) + Math.Abs(p.Y - to.Y);
        public static Point XDir(this Point p) => new Point(Math.Sign(p.X), 0);
        public static Point YDir(this Point p) => new Point(0, Math.Sign(p.Y));
    }

    public class Planet
    {
        public readonly PlanetSimulationConfig Config;

        public int CurrentStep { get; private set; }
        public int MaxGeneration { get; private set; }
        public int StarvationDeaths { get; private set; }
        public int AgeDeaths { get; private set; }

        public double AvgPopAgeFraction => Population.Count == 0 ? 0 : Population.Average(p => p.AgeFraction);
        public double AvgBushAgeFraction => Bushes.Count == 0 ? 0 : Bushes.Average(b => b.AgeFraction);

        public int TotalFertility
        {
            get
            {
                var result = 0;
                for(int x = 0; x < Config.Width; ++x)
                    for (int y = 0; y < Config.Height; ++y)
                        result += _fertility[x, y];
                return result;
            }
        }

        public List<Pop> Population;
        public List<Bush> Bushes;

        public Random Random;

        private List<Pop> _newPops;
        private List<Bush> _newBushes;
        private int[,] _fertility;

        public Planet(PlanetSimulationConfig config)
        {
            Config = config;

            Random = new Random();

            Population = new List<Pop>();
            Bushes = new List<Bush>();
            _fertility = new int[Config.Width, Config.Height];

            _newPops = new List<Pop>();
            _newBushes = new List<Bush>();
        }

        public bool IsInBounds(Point p) => p.X >= 0 && p.X < Config.Width && p.Y >= 0 && p.Y <= Config.Height;

        public void InitRandom()
        {
            Population.Clear();
            Bushes.Clear();

            for (int x = 0; x < Config.Width; ++x)
            {
                for (int y = 0; y < Config.Height; ++y)
                {
                    _fertility[x, y] = Random.Next(Config.FertilityMinStart, Config.FertilityMaxStart + 1);
                }
            }

            for (int p = 0; p < Config.PopStartAmount; ++p)
            {
                Population.Add(new Pop(this, Random.NextPoint(0, 0, Config.Width, Config.Height)));
            }

            for (int b = 0; b < Config.BushStartAmount; ++b)
            {
                var bush = new Bush(this, Random.NextPoint(0, 0, Config.Width, Config.Height))
                {
                    Age = Random.Next(Config.BushMaxAge)
                };
                Bushes.Add(bush);

                var growPosCross = GetCross(bush.Position);
                growPosCross.ForEach(p => RemoveFertility(p, Config.BushGrowCost));
            }
        }

        public void AddPop(Point position, int generation)
        {
            MaxGeneration = Math.Max(MaxGeneration, generation);
            _newPops.Add(new Pop(this, position, generation));
        }

        public void AddBush(Point position)
        {
            _newBushes.Add(new Bush(this, position));
        }

        public void Step()
        {
            CurrentStep++;

            // Remove aged and starved pops
            var deadPops = Population.FindAll(p => p.Age >= Config.PopMaxAge);
            AgeDeaths += deadPops.Count;
            var starvedPops = Population.FindAll(p => p.Fullness <= 0);
            StarvationDeaths += starvedPops.Count;
            deadPops.Concat(starvedPops).ToList().ForEach(p =>
            {
                Population.Remove(p);
                //GetCross(p.Position).ForEach(pos => AddFertility(pos, Config.PopDeathFertility));
                AddFertility(p.Position, p.GetFood(FoodTypes.Berry) + p.DigestionAmount);
            });
            // Add newborn pops
            Population = Population.Concat(_newPops).ToList();
            _newPops.Clear();

            // Remove aged bushes
            var deadBushes = Bushes.FindAll(b => b.Age >= Config.BushMaxAge || b.Age >= Config.BushMaxGrowAge && b.Berries < 0);
            deadBushes.ForEach(b =>
            {
                Bushes.Remove(b);
                GetCross(b.Position).ForEach(p => AddFertility(p, Config.BushDeathFertility));
            });
            // Add newly grown bushes
            Bushes = Bushes.Concat(_newBushes).ToList();
            _newBushes.Clear();

            Population.ForEach(p => p.Step());
            Bushes.ForEach(b => b.Step());
        }

        public List<Point> GetCross(Point position)
        {
            var result = new List<Point>
            {
                position,
                position + new Point(1, 0),
                position + new Point(0, 1),
                position + new Point(-1, 0),
                position + new Point(0, -1),
            };
            result.RemoveAll(p => p.X < 0 || p.X >= Config.Width || p.Y < 0 || p.Y >= Config.Height);
            return result;
        }

        public void TrySeedBushCross(Point position)
        {
            if (Random.NextFloat() < Config.BushGrowChance)
            {
                var positions = GetCross(position);
                positions.RemoveAll(p => Bushes.Any(b => b.Position == p));
                var fertileTiles = positions.Where(p => GetFertility(p) >= Config.BushGrowCost).ToList();
                if (fertileTiles.Count == 0) return;
                var growPos = Random.Choose<Point>(fertileTiles);
                if(SpendFertility(growPos, Config.BushGrowCost)) AddBush(growPos);
            }
        }

        public void AddFertility(int x, int y, int amount)
        {
            if (amount < 0) throw new ArgumentException("Added fertility amount should not be negative.");
            _fertility[x, y] += amount;
        }

        public void AddFertility(Point position, int amount) => AddFertility(position.X, position.Y, amount);

        public bool SpendFertility(int x, int y, int amount)
        {
            if(amount < 0) throw new ArgumentException("Removed fertility amount should not be negative.");
            if (_fertility[x, y] >= amount)
            {
                _fertility[x, y] -= amount;
                return true;
            }
            return false;
        }

        public bool SpendFertility(Point position, int amount) => SpendFertility(position.X, position.Y, amount);

        public void RemoveFertility(int x, int y, int amount) => _fertility[x, y] = Math.Max(0, _fertility[x, y] - amount);

        public void RemoveFertility(Point position, int amount) => RemoveFertility(position.X, position.Y, amount);

        public int GetFertility(int x, int y) => _fertility[x, y];

        public int GetFertility(Point position) => _fertility[position.X, position.Y];
    }
}
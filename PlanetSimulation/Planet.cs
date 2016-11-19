using System;
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

        public readonly int Width;
        public readonly int Height;

        public List<Pop> Population;
        public List<Bush> Bushes;

        private List<Pop> _newPops;
        public Random Random;

        public Planet(int width, int height, PlanetSimulationConfig config)
        {
            Width = width;
            Height = height;
            Config = config;

            Random = new Random();

            Population = new List<Pop>();
            Bushes = new List<Bush>();

            _newPops = new List<Pop>();
        }

        public void InitRandom(int pops, int bushes)
        {
            Population.Clear();
            Bushes.Clear();

            for (int p = 0; p < pops; ++p)
            {
                Population.Add(new Pop(this, Random.NextPoint(0, 0, Width, Height)));
            }

            for (int b = 0; b < bushes; ++b)
            {
                Bushes.Add(new Bush(this, Random.NextPoint(0, 0, Width, Height)));
            }
        }

        public void AddPop(Point position, int generation)
        {
            MaxGeneration = Math.Max(MaxGeneration, generation);
            _newPops.Add(new Pop(this, position, generation));
        }

        public void Step()
        {
            CurrentStep++;
            Population.RemoveAll(p => p.Age >= Config.PopMaxAge || p.Fullness <= 0);
            Population = Population.Concat(_newPops).ToList();
            _newPops.Clear();
            Population.ForEach(p => p.Step());
            Bushes.ForEach(b => b.Step());
            
            var newBushes = Random.Next(5);
            for (int i = 0; i < newBushes; i++)
            {
                Bushes.Add(new Bush(this, Random.NextPoint(0, 0, Width, Height)));
            }
        }
    }

    public struct PlanetSimulationConfig
    {
        public float BushBerryGrowChance;
        public int BushMaxBerries;
        public int BushStartingBerries;
        public int PopBerryConsumption;
        public int PopForagingRange;
        public int PopStartingFullness;
        public int PopMaxAge;
        public int PopBreedingAge;
        public int PopBreedingFullness;
        public int PopBreedingCost;

        public static PlanetSimulationConfig DefaultConfig => new PlanetSimulationConfig()
        {
            BushBerryGrowChance = 0.15f,
            BushMaxBerries = 15,
            BushStartingBerries = 3,
            PopBerryConsumption = 3,
            PopForagingRange = 15,
            PopStartingFullness = 25,
            PopMaxAge = 60,
            PopBreedingAge = 15,
            PopBreedingFullness = 15,
            PopBreedingCost = 5,
        };
    }
}
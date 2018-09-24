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

	public struct BushDistance
	{
		public Bush Bush;
		public int Distance;

		public BushDistance(Bush bush, int distance)
		{
			Bush=bush;
			Distance=distance;
		}
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
		public Color GetFertilityColor(int fertility) => Color.Lerp(_lowFertilityColor, _highFertilityColor, fertility/MaxFertility);
		public int MaxFertility { get; private set; }

		private Color _lowFertilityColor = Color.SandyBrown;
		private Color _highFertilityColor = Color.SaddleBrown*0.5f;

		public int TotalFertility
		{
			get
			{
				var result = 0;
				for(int x = 0; x < Config.Width; x++)
					for(int y = 0; y < Config.Height; y++)
						result += _fertility[x, y];
				return result;
			}
		}

		public int CurrentMaxFertility
		{
			get
			{
				var result = -1;
				for(int x = 0; x < Config.Width; x++)
					for(int y = 0; y < Config.Height; y++)
						result = Math.Max(result, _fertility[x, y]);
				return result;
			}
		}

		public List<Pop> Population;
        public List<Bush> Bushes;
		public List<BushDistance>[,] BushesInRange;

        public Random Random;

        private List<Pop> _newPops;
        private List<Bush> _newBushes;
        private int[,] _fertility;

        public Planet(PlanetSimulationConfig config)
        {
            Config = config;
			MaxFertility = (int)Math.Pow(Config.FertilityMaxStart, Config.FertilityAreaPower);

			Random = new Random();

            Population = new List<Pop>();
            Bushes = new List<Bush>();
			BushesInRange = new List<BushDistance>[Config.Width, Config.Height];
			for(var x = 0; x < Config.Width; x++) for(var y = 0; y < Config.Height; y++) BushesInRange[x, y] = new List<BushDistance>(20);
            _fertility = new int[Config.Width, Config.Height];

            _newPops = new List<Pop>();
            _newBushes = new List<Bush>();
        }

        public bool IsInBounds(Point p) => p.X >= 0 && p.X < Config.Width && p.Y >= 0 && p.Y < Config.Height;
		public bool NotInBounds(Point p) => !IsInBounds(p);

        public void Init()
		{
			Population.Clear();
			Bushes.Clear();

			InitFertilityRandom();
			if(Config.FertilityInitHalfArea > 0)
			{
				FertilityAverageAreaPower(Config.FertilityInitHalfArea, Config.FertilityAreaPower);
			}

			if(Config.PopSpawnSize > 0)
			{
				var startPos = Random.NextPoint(0, 0, Config.Width - Config.PopSpawnSize, Config.Height - Config.PopSpawnSize);
				for(int p = 0; p < Config.PopStartAmount; p++)
				{
					var popPosition = startPos + Random.NextPoint(0, 0, Config.PopSpawnSize, Config.PopSpawnSize);
					Population.Add(new Pop(this, popPosition));
				}
			}
			else
			{
				for(int p = 0; p < Config.PopStartAmount; p++)
				{
					Population.Add(new Pop(this, Random.NextPoint(0, 0, Config.Width, Config.Height)));
				}
			}

			for(int b = 0; b < Config.BushStartAmount; b++)
			{
				var bush = new Bush(this, Random.NextPoint(0, 0, Config.Width - 1, Config.Height - 1));
				Bushes.Add(bush);
				AddBushRange(bush);
			}
		}

		private void InitFertilityRandom()
		{
			for(int x = 0; x < Config.Width; x++)
			{
				for(int y = 0; y < Config.Height; y++)
				{
					_fertility[x, y] = Random.Next(Config.FertilityMinStart, Config.FertilityMaxStart + 1);
				}
			}
		}

		private void FertilitySquared()
		{
			for(int x = 0; x < Config.Width; x++)
			{
				for(int y = 0; y < Config.Height; y++)
				{
					_fertility[x, y] = _fertility[x, y]*_fertility[x, y];
				}
			}
		}

		private void FertilityAverageAreaPower(int halfArea, float power)
		{
			var totals = new int[Config.Width, Config.Height];
			var actualArea = new float[Config.Width, Config.Height];

			for(int x = 0; x < Config.Width; x++)
			{
				for(int y = 0; y < Config.Height; y++)
				{
					for(var x2 = x - halfArea; x2 < x + halfArea; x2++)
					{
						if(x2 < 0 || x2 >= Config.Width) continue;
						for(var y2 = y - halfArea; y2 < y + halfArea; y2++)
						{
							if(y2 < 0 || y2 >= Config.Height) continue;
							totals[x, y] += _fertility[x2, y2];
							actualArea[x, y] += 1f;
						}
					}
				}
			}
			for(int x = 0; x < Config.Width; x++)
			{
				for(int y = 0; y < Config.Height; y++)
				{
					_fertility[x, y] = (int)Math.Pow(totals[x, y]/actualArea[x, y], power);
				}
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

		private void AddBushRange(Bush bush)
		{
			int range = Config.PopVisionRange;
			var startX = Math.Max(0, bush.Position.X - range);
			var endX = Math.Min(Config.Width - 1, bush.Position.X + range);
			var startY = Math.Max(0, bush.Position.Y - range);
			var endY = Math.Min(Config.Height - 1, bush.Position.Y + range);

			for(var x = startX; x < endX; x++)
			{
				for(var y = startY; y < endY; y++)
				{
					var pos = new Point(x, y);
					var bd = new BushDistance(bush, pos.Manhattan(bush.Position));
					if(bd.Distance <= range)
					{
						BushesInRange[x, y].Add(bd);
					}
				}
			}
		}

		private void RemoveBushRange(Bush bush)
		{
			int range = Config.PopVisionRange;
			var startX = Math.Max(0, bush.Position.X - range);
			var endX = Math.Min(Config.Width - 1, bush.Position.X + range);
			var startY = Math.Max(0, bush.Position.Y - range);
			var endY = Math.Min(Config.Height - 1, bush.Position.Y + range);

			for(var x = startX; x < endX; x++)
			{
				for(var y = startY; y < endY; y++)
				{
					var bdIndex = BushesInRange[x, y].FindIndex(bd => bd.Bush == bush);
					if(bdIndex != -1) BushesInRange[x, y].RemoveAt(bdIndex);
				}
			}
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
                AddFertility(p.Position, p.GetFood(FoodTypes.Berry)*Config.BushBerryGrowCost + p.DigestionAmount);
            });
            // Add newborn pops
            Population = Population.Concat(_newPops).ToList();
            _newPops.Clear();

            // Remove aged bushes
            var deadBushes = Bushes.FindAll(b => b.Age >= Config.BushMaxAge);
            deadBushes.ForEach(b =>
            {
				Bushes.Remove(b);
				int fertility = Config.BushDeathFertility + b.Berries*Config.BushBerryGrowCost;
				var cross = GetCross(b.Position);//.ForEach(p => AddFertility(p, fertility));
				for(var i = 0; i < fertility; i++)
				{
					AddFertility(Random.Choose<Point>(cross), 1);
				}
				RemoveBushRange(b);
            });
            // Add newly grown bushes
            Bushes = Bushes.Concat(_newBushes).ToList();
			_newBushes.ForEach(AddBushRange);
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
            result.RemoveAll(NotInBounds);
            return result;
        }

        public void TrySeedBushCross(Point position)
        {
            if (Bushes.Count > Config.BushLimit) return;
            if (Random.NextFloat() < Config.BushGrowChance)
            {
                var positions = GetCross(position);
                positions.RemoveAll(p => Bushes.Any(b => b.Position == p));
                var fertileTiles = positions.Where(p => GetFertility(p) >= Config.BushGrowCost).ToList();
                if (fertileTiles.Count == 0) return;
                var roomTiles = fertileTiles/*.Where(ft => Bushes.All(b => b.Position == position || b.Position.Manhattan(ft) > 2)).ToList()*/;
                if (roomTiles.Count == 0) return;
                var growPos = Random.Choose<Point>(roomTiles);
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
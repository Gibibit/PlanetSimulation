namespace PlanetSimulation
{
    public struct PlanetSimulationConfig
	{
		public int Width;
		public int Height;
		public int PopSpawnSize;
		public float BushBerryGrowChance;
        public float BushGrowChance;
        public int BushBerryGrowCost;
        public int BushDeathFertility;
		public int BushGrowCost;
        public int BushGrowDelay;
        public int BushLimit;
        public int BushMaxAge;
        public int BushMaxBerries;
        public int BushMaxGrowAge;
        public int BushStartAmount;
        public int BushStartingBerries;
        public int FertilityMaxStart;
        public int FertilityMinStart;
		public int FertilityInitHalfArea;
		public float FertilityAreaPower;
		public float PopBreedingDesireChance;
		public float PopWanderTurnChance;
		public int PopBerryDigestDelayMax;
        public int PopBerryDigestDelayMin;
        public int PopBerryHarvestAmount;
        public int PopBreedingAge;
        public int PopBreedingCost;
        public int PopBreedingDelay;
        public int PopBreedingFullness;
        public int PopBreedingRange;
        public int PopEatAmount;
        public int PopFoodHeuristicRandom;
        public int PopFoodStockpileMax;
        public int PopFoodStockpileMin;
        public int PopForagingHunger;
        public int PopFullnessConsumption;
        public int PopFullnessHungry;
        public int PopHarvestRange;
        public int PopLimit;
        public int PopMaxAge;
        public int PopPoopFertility;
        public int PopStartAmount;
        public int PopStartingFullness;
        public int PopVisionRange;
        public int PopWalkDelay;
        public int PopWanderFitnessThreshold;

        public static PlanetSimulationConfig DefaultConfig => new PlanetSimulationConfig()
		{
			Width = 55,
			Height = 30,
			PopSpawnSize = 6,
			BushBerryGrowChance = 0.25f,
            BushBerryGrowCost = 1,
            BushDeathFertility = 4,
            BushGrowChance = 0.04f,
            BushGrowCost = 7,
            BushGrowDelay = 100,
            BushLimit = 200,
            BushMaxAge = 800,
            BushMaxBerries = 15,
            BushMaxGrowAge = 730,
            BushStartAmount = 25,
            BushStartingBerries = 3,
            FertilityMaxStart = 25,
            FertilityMinStart = 0,
			FertilityInitHalfArea = 2,
			FertilityAreaPower = 2f,
            PopBerryDigestDelayMax = 40,
            PopBerryDigestDelayMin = 30,
            PopBerryHarvestAmount = 1,
            PopBreedingAge = 400,
            PopBreedingCost = 60,
            PopBreedingDelay = 100,
            PopBreedingDesireChance = 0.003f, // Chance each step that the pop desires a child from then on out
            PopBreedingFullness = 200,
            PopBreedingRange = 1,
            PopEatAmount = 4,
            PopFoodHeuristicRandom = 2,
            PopFoodStockpileMax = 24, // Amount of food that pop tries to reach once they start foraging
            PopFoodStockpileMin = 4, // Minimum amount of food that the pop desires to have
            PopForagingHunger = 300,
            PopFullnessConsumption = 1,
            PopFullnessHungry = 200,
            PopHarvestRange = 1,
            PopLimit = 150,
            PopMaxAge = 2000,
            PopPoopFertility = 1,
            PopStartAmount = 10,
            PopStartingFullness = 300,
            PopVisionRange = 25,
            PopWalkDelay = 3,
            PopWanderFitnessThreshold = 5, // If there are no heuristics with fitness > threshold, pops wander
            PopWanderTurnChance = 0.25f
        };
    }
}
namespace PlanetSimulation
{
    public struct PlanetSimulationConfig
    {
        public int BushGrowCost;
        public int BushDeathFertility;
        public float BushGrowChance;
        public int BushMaxAge;
        public int BushMaxGrowAge;
        public int BushBerryGrowCost;
        public float BushBerryGrowChance;
        public int BushMaxBerries;
        public int BushStartingBerries;
        public int PopBerryHarvestAmount;
        public int PopVisionRange;
        public int PopStartingFullness;
        public int PopMaxAge;
        public int PopBreedingAge;
        public int PopBreedingFullness;
        public int PopBreedingCost;
        public int PopWalkDelay;
        public int PopFullnessHungry;
        public int FertilityMinStart;
        public int FertilityMaxStart;
        public int PopBerryDigestDelayMin;
        public int PopEatAmount;
        public int PopFoodHeuristicRandom;
        public int BushGrowDelay;
        public float PopWanderTurnChance;
        public int PopWanderFitnessThreshold;
        public int PopStartAmount;
        public int BushStartAmount;
        public int Width;
        public int Height;
        public int PopPoopFertility;
        public int PopBerryDigestDelayMax;
        public int PopFullnessConsumption;
        public int PopBreedingRange;
        public int PopHarvestRange;
        public int PopForagingHunger;
        public int PopFoodStockpileMin;
        public float PopBreedingDesireChance;
        public int PopBreedingDelay;
        public int PopFoodStockpileMax;
        public int BushLimit;
        public int PopLimit;

        public static PlanetSimulationConfig DefaultConfig => new PlanetSimulationConfig()
        {
            Width = 50,
            Height = 25,
            PopStartAmount = 10,
            PopLimit = 150,
            BushStartAmount = 20,
            BushLimit = 200,
            FertilityMinStart = 1,
            FertilityMaxStart = 80,
            BushGrowCost = 7,
            BushDeathFertility = 1,
            BushGrowChance = 0.04f,
            BushGrowDelay = 100,
            BushMaxAge = 800,
            BushMaxGrowAge = 730,
            BushBerryGrowCost = 1,
            BushBerryGrowChance = 0.25f,
            BushMaxBerries = 15,
            BushStartingBerries = 3,
            PopBerryHarvestAmount = 1,
            PopHarvestRange = 1,
            PopVisionRange = 25,
            PopStartingFullness = 300,
            PopMaxAge = 2000,
            PopBreedingAge = 400,
            PopBreedingFullness = 200,
            PopBreedingCost = 60,
            PopBreedingRange = 1,
            PopBreedingDelay = 100,
            PopBreedingDesireChance = 0.003f, // Chance each step that the pop desires a child from then on out
            PopWalkDelay = 3,
            PopFullnessHungry = 200,
            PopEatAmount = 4,
            PopBerryDigestDelayMin = 30,
            PopBerryDigestDelayMax = 40,
            PopPoopFertility = 1,
            PopFoodHeuristicRandom = 2,
            PopWanderTurnChance = 0.25f,
            PopWanderFitnessThreshold = 5, // If there are no heuristics with fitness > threshold, pops wander
            PopFullnessConsumption = 1,
            PopForagingHunger = 300,
            PopFoodStockpileMin = 4, // Minimum amount of food that the pop desires to have
            PopFoodStockpileMax = 24, // Amount of food that pop tries to reach once they start foraging
        };
    }
}
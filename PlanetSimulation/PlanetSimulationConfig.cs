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
        public int PopDeathFertility;
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

        public static PlanetSimulationConfig DefaultConfig => new PlanetSimulationConfig()
        {
            Width = 25,
            Height = 15,
            PopStartAmount = 4,
            BushStartAmount = 15,
            FertilityMinStart = 1,
            FertilityMaxStart = 80,
            BushGrowCost = 7,
            BushDeathFertility = 1,
            BushGrowChance = 0.04f,
            BushGrowDelay = 100,
            BushMaxAge = 400,
            BushMaxGrowAge = 330,
            BushBerryGrowCost = 1,
            BushBerryGrowChance = 0.25f,
            BushMaxBerries = 15,
            BushStartingBerries = 3,
            PopBerryHarvestAmount = 1,
            PopVisionRange = 10,
            PopStartingFullness = 250,
            PopMaxAge = 600,
            PopDeathFertility = 1,
            PopBreedingAge = 100,
            PopBreedingFullness = 200,
            PopBreedingCost = 60,
            PopWalkDelay = 4,
            PopFullnessHungry = 150,
            PopEatAmount = 8,
            PopBerryDigestDelayMin = 12,
            PopBerryDigestDelayMax = 18,
            PopPoopFertility = 1,
            PopFoodHeuristicRandom = 2,
            PopWanderTurnChance = 0.25f,
            PopWanderFitnessThreshold = 5, // If there are no heuristics with fitness > threshold, pops wander
            PopFullnessConsumption = 2,
        };
    }
}
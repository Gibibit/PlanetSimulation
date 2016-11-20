namespace PlanetSimulation
{
    public struct PlanetSimulationConfig
    {
        public int BushMaxGrow;
        public float BushGrowChance;
        public int BushMaxAge;
        public int BushMaxGrowAge;
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

        public static PlanetSimulationConfig DefaultConfig => new PlanetSimulationConfig()
        {
            BushMaxGrow = 5,
            BushGrowChance = 0.1f,
            BushMaxAge = 280,
            BushMaxGrowAge = 200,
            BushBerryGrowChance = 0.25f,
            BushMaxBerries = 15,
            BushStartingBerries = 3,
            PopBerryHarvestAmount = 1,
            PopVisionRange = 25,
            PopStartingFullness = 250,
            PopMaxAge = 600,
            PopBreedingAge = 100,
            PopBreedingFullness = 150,
            PopBreedingCost = 90,
            PopWalkDelay = 4,
            PopFullnessHungry = 120,
        };
    }
}
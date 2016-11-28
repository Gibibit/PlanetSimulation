namespace PlanetSimulation
{
    public struct Food
    {
        public FoodTypes FoodType;
        public int Amount;

        public Food(FoodTypes type, int amount)
        {
            FoodType = type;
            Amount = amount;
        }
    }

    public enum FoodTypes
    {
        Berry
    }

    public static class FoodType
    {
        public static int Power(FoodTypes type)
        {
            switch (type)
            {
                case FoodTypes.Berry: return 8;
                default: return 0;
            }
        }
    }
}
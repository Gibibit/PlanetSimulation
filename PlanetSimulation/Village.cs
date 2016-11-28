using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PlanetSimulation
{
    public class Village : SurfaceObject {

        private Food[] _storedFood;
        private List<Pop> _pops;

        public Village(Planet planet, Point position) : base(planet, position)
        {
            _pops = new List<Pop>();
            _storedFood = new Food[Enum.GetValues(typeof(FoodTypes)).Length];
            for (int i = 0; i < _storedFood.Length; i++)
            {
                _storedFood[i] = new Food((FoodTypes) i, 0);
            }
        }

        public void StoreFood(Food food)
        {
            _storedFood[(int) food.FoodType].Amount += food.Amount;
        }

        public void Step()
        {
            
        }

    }
}
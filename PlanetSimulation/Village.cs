using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PlanetSimulation
{
    public class Village : SurfaceObject {

        public int StoredFood;
        private List<Pop> _pops;

        public Village(Planet planet, Point position) : base(planet, position)
        {
            _pops = new List<Pop>();
        }

    }
}
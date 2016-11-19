using Microsoft.Xna.Framework;

namespace PlanetSimulation
{
    public class SurfaceObject
    {
        public Planet Planet;
        public Point Position;

        public SurfaceObject(Planet planet, Point position)
        {
            Planet = planet;
            Position = position;
        }
    }
}
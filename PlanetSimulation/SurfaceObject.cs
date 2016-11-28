using Microsoft.Xna.Framework;

namespace PlanetSimulation
{
    public class SurfaceObject
    {
        public Planet Planet;
        private Point _position;

        public Point Position
        {
            get { return _position; }
            set
            {
                _position = value;
                _position.X = MathHelper.Clamp(_position.X, 0, Planet.Config.Width - 1);
                _position.Y = MathHelper.Clamp(_position.Y, 0, Planet.Config.Height - 1);
            }
        }

        public SurfaceObject(Planet planet, Point position)
        {
            Planet = planet;
            Position = position;
        }
    }
}
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

    public class Planet
    {
        public readonly int Width;
        public readonly int Height;

        public List<Pop> Population;
        public List<Bush> Bushes;

        public Planet(int width, int height)
        {
            Width = width;
            Height = height;

            Population = new List<Pop>();
            Bushes = new List<Bush>();
        }

        public void InitRandom(int pops, int bushes)
        {
            Population.Clear();
            Bushes.Clear();

            var r = new Random();

            for (int p = 0; p < pops; ++p)
            {
                Population.Add(new Pop(this, r.NextPoint(0, 0, Width, Height)));
            }

            for (int b = 0; b < bushes; ++b)
            {
                Bushes.Add(new Bush(this, r.NextPoint(0, 0, Width, Height)));
            }
        }

        public void Step()
        {
            Population.ForEach(p => p.Step());
            Bushes.ForEach(b => b.Step());
        }
    }

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

    public class Pop : SurfaceObject
    {
        public int Hunger = 0;
        public int Age = 0;
        public int ForagingRange = 5;

        private readonly Random _random;

        public Pop(Planet planet, Point position) : base(planet, position)
        {
            _random = new Random();
        }

        public void Step()
        {
            var bush = Planet.Bushes.Find(b => b.Position == Position);
            if (bush != null)
            {
                bush.Berries--;
                if(bush.Berries == 0) Planet.Bushes.Remove(bush);
                Hunger++;
            }
            else
            {
                // Find bushes within foraging range
                var bushes = Planet.Bushes.FindAll(b => b.Position.Manhattan(Position) <= ForagingRange);
                if (bushes.Count > 0)
                {
                    // Find closest bush
                    var closest = bushes.Min(b => b.Position.Manhattan(Position));
                    // Remove farther away bushes
                    bushes.RemoveAll(b => b.Position.Manhattan(Position) > closest);
                    // Choose randomly from remaining bushes
                    var target = _random.Choose<Bush>(bushes);
                    // Navigate towards bush
                    var dir = target.Position - Position;
                    var movement = Point.Zero;
                    if (dir.X != 0 && dir.Y != 0) movement = _random.Choose(dir.XDir(), dir.YDir());
                    else if (dir.X != 0) movement = dir.XDir();
                    else if (dir.Y != 0) movement = dir.YDir();

                    Position += movement;
                }
            }
        }
    }

    public class Bush : SurfaceObject
    {
        public int Berries = 1;

        private Random _random;

        public Bush(Planet planet, Point position) : base(planet, position)
        {
            _random = new Random();
        }

        public void Step()
        {
            if (_random.NextFloat() > 0.8f) Berries++;
        }
    }
}

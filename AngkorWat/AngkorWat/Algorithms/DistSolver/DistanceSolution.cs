using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.DistSolver
{
    internal interface IPunkt
    {
        public double X { get; }
        public double Y { get; }
    }
    internal interface ILocation : IPunkt
    {
        public int LocationId { get; }
        public bool IsSanta { get; }
        public DerPunkt AsPunkt();
    }
    internal class Santa : ILocation
    {
        public double X => 0;
        public double Y => 0;
        public bool IsSanta => true;
        public int LocationId => 0;
        public Santa()
        {

        }
        public DerPunkt AsPunkt()
        {
            return new DerPunkt { X = X, Y = Y };
        }
    }

    internal class DerPunkt : IPunkt
    {
        public double X { get; init; }
        public double Y { get; init; }
    }

    internal class Route
    {
        public ILocation From { get; }
        public ILocation To { get; }
        /// <summary>
        /// Время в пути в секундах
        /// </summary>
        public double TravelTime { get; set; }
        /// <summary>
        /// Маршрут движения из From в To
        /// </summary>
        public List<DerPunkt> Punkts { get; set; }
        public Route(ILocation from, ILocation to)
        {
            From = from;
            To = to;

            Debug.Assert(from != to);

            Punkts = new List<DerPunkt>()
            {
                from.AsPunkt(),
                to.AsPunkt(),
            };

            TravelTime = 0.0d;
        }

        internal Route AsReverse()
        {
            var route = new Route(To, From)
            {
                TravelTime = TravelTime,
                Punkts = Punkts.ToList(),
            };

            route.Punkts.Reverse();

            return route;
        }
    }

    internal class DistanceSolution
    {
        public Dictionary<(ILocation, ILocation), Route> Routes { get; set; } 
        public DistanceSolution()
        {
            Routes = new();
        }
    }
}

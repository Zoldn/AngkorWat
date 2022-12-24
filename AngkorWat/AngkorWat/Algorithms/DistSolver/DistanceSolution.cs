using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.DistSolver
{
    internal enum PunktType
    {
        SANTA = 0,
        CHILD,
        FREE,
        SNOWAREA,
    }
    internal interface IPunkt
    {
        public int X { get; }
        public int Y { get; }
        public PunktType PunktType { get; }
    }
    internal interface ILocation : IPunkt
    {
        public int LocationId { get; }
        public bool IsSanta { get; }
        public DerPunkt AsPunkt();
    }
    internal class Santa : ILocation
    {
        public int X => 0;
        public int Y => 0;
        public bool IsSanta => true;
        public PunktType PunktType => PunktType.SANTA;
        public int LocationId => 0;
        public Santa()
        {

        }
        public DerPunkt AsPunkt()
        {
            return new DerPunkt { X = X, Y = Y, PunktType = PunktType };
        }
        public override string ToString()
        {
            return $"Santa home at {X}, {Y}";
        }
    }

    internal class DerPunkt : IPunkt
    {
        public PunktType PunktType { get; init; }
        public int X { get; init; }
        public int Y { get; init; }
        public DerPunkt() { }
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
        public List<IPunkt> Punkts { get; set; }
        /// <summary>
        /// Длина маршрута в км
        /// </summary>
        public double Distance { get; set; }

        public Route(ILocation from, ILocation to)
        {
            From = from;
            To = to;

            Debug.Assert(from != to);

            Punkts = new List<IPunkt>()
            {
                from.AsPunkt(),
                to.AsPunkt(),
            };

            TravelTime = 0.0d;
            Distance = 0.0d;
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

using AngkorWat.Components;
using AngkorWat.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.DistSolver.PathFindingStrategies
{
    internal class Leg
    {
        public IPunkt From { get; }
        public IPunkt To { get; }
        /// <summary>
        /// Находится ли стартовая точка в буре, null, если нет
        /// </summary>
        public SnowArea? FromSnowArea { get; set; }
        /// <summary>
        /// Находится ли конечная точка в буре, null, если нет
        /// </summary>
        public SnowArea? ToSnowArea { get; set; }
        /// <summary>
        /// Пересекаемые бури, которые не являются начальными и стартовыми,
        /// упорядоченные от начала отрезка к концу
        /// </summary>
        public List<SnowArea> CrossSnowAreas { get; set; }
        public bool IsCrossingLeg => FromSnowArea == null
            && ToSnowArea == null
            && CrossSnowAreas.Any();

        public Leg(IPunkt from, IPunkt to)
        {
            From = from;
            To = to;

            CrossSnowAreas = new();
        }
    }

    internal class GreedPathFinding : IPathFindingStrategy
    {
        public static readonly List<int> SIGNS = new()
        {
            +1,
            -1,
        };

        private static readonly double ARC_SIZE = Math.PI / 20;
        private static readonly double RADIUS_EXPAND = 1.1d;

        protected AllData allData;
        public GreedPathFinding(AllData allData)
        {
            this.allData = allData;
        }
        public void Calculate(Route route)
        {
            var legs = new List<Leg>()
            {
                new Leg(route.From, route.To)
            };

            int iteration = 0;

            while (true)
            {
                iteration++;

                if (iteration > 50)
                {
                    throw new StackOverflowException();
                }

                if (true)
                {
                    var tunkts = legs
                        .Select(e => e.From)
                        .Append(route.To)
                        .Select(e => new move() 
                        {
                            x = e.X,
                            y = e.Y
                        })
                        .ToList();

                    var json = JsonConvert.SerializeObject(tunkts);
                }

                CheckCrossesWithSnowAreas(legs);

                var targetCrossedLeg = legs
                    .FirstOrDefault(leg => leg.IsCrossingLeg);

                if (targetCrossedLeg != null)
                {
                    legs = SplitCrossingLeg(legs, targetCrossedLeg);

                    continue;
                }

                break;
            }

            route.Punkts = legs
                .Select(e => e.From)
                .Append(route.To)
                .ToList();

            CheckDuplicates(route);

            CalculateDistance(route);
        }

        private void CheckDuplicates(Route route)
        {
            var newPunkts = new List<IPunkt>() 
            {
                route.From,
            };

            for (int i = 0; i < route.Punkts.Count - 1; i++)
            {
                var from = route.Punkts[i];
                var to = route.Punkts[i + 1];

                if (from.X == to.X && from.Y == to.Y)
                {
                    continue;
                }

                newPunkts.Add(to);
            }

            route.Punkts = newPunkts;
        }

        public void CalculateDistance(Route route)
        {
            route.TravelTime = 0.0d;

            for (int i = 0; i < route.Punkts.Count - 1; i++)
            {
                var from = route.Punkts[i];
                var to = route.Punkts[i + 1];

                var localTotalLenght = GeometryUtils.GetDistance(from, to);
                var localSnowLength = 0.0d;

                foreach (var snowArea in allData.SnowAreas)
                {
                    localSnowLength += GeometryUtils.GetOverlapWithSnowArea(snowArea, from, to);
                }

                var localCleanLength = localTotalLenght - localSnowLength;

                route.TravelTime += localCleanLength / allData.AirSpeed + localSnowLength / allData.SnowSpeed;
                route.Distance += localTotalLenght;
            }
        }

        private List<Leg> SplitCrossingLeg(List<Leg> legs, Leg targetCrossedLeg)
        {
            var snowArea = targetCrossedLeg.CrossSnowAreas.First();

            var angleFrom = GeometryUtils.GetPolarAngle(snowArea, targetCrossedLeg.From);
            var angleTo = GeometryUtils.GetPolarAngle(snowArea, targetCrossedLeg.To);

            var tangentAngleFrom = Math.Acos(snowArea.R / GeometryUtils.GetDistance(snowArea, targetCrossedLeg.From));
            var tangentAngleTo = Math.Acos(snowArea.R / GeometryUtils.GetDistance(snowArea, targetCrossedLeg.To));

            var sideVariants = new Dictionary<int, List<IPunkt>>();

            foreach (var sign in SIGNS)
            {
                List<double> arcNodes = new();

                var fromAngle = GeometryUtils.SumPolarAngles(angleFrom, sign * tangentAngleFrom);
                var toAngle = GeometryUtils.SumPolarAngles(angleTo, -sign * tangentAngleTo);

                double startAngle;
                double step;
                int arcs;
                bool isFromStarting = true;

                if (Math.Abs(fromAngle - toAngle) <= Math.PI)
                {
                    arcs = (int)Math.Ceiling(Math.Abs(fromAngle - toAngle) / ARC_SIZE);

                    startAngle = Math.Min(fromAngle, toAngle);

                    isFromStarting = startAngle == fromAngle;

                    step = Math.Abs(fromAngle - toAngle) / arcs;
                }
                else
                {
                    arcs = (int)Math.Ceiling((2 * Math.PI - Math.Abs(fromAngle - toAngle)) / ARC_SIZE);

                    startAngle = Math.Max(fromAngle, toAngle);

                    isFromStarting = startAngle == fromAngle;

                    step = (2 * Math.PI - Math.Abs(fromAngle - toAngle)) / arcs;
                }

                for (int i = 0; i <= arcs; i++)
                {
                    arcNodes.Add(GeometryUtils.SumPolarAngles(startAngle, step * i));
                }

                var arcPunkts = arcNodes
                    .Select(n => new DerPunkt()
                    {
                        PunktType = PunktType.FREE,
                        X = (int)Math.Round(snowArea.X + snowArea.R * RADIUS_EXPAND * Math.Cos(n)),
                        Y = (int)Math.Round(snowArea.Y + snowArea.R * RADIUS_EXPAND * Math.Sin(n)),
                    })
                    .ToList();

                if (!isFromStarting)
                {
                    arcPunkts.Reverse();
                }

                var newPunkts = new List<IPunkt>() { targetCrossedLeg.From }
                    .Concat(arcPunkts)
                    .Append(targetCrossedLeg.To)
                    .ToList();

                sideVariants.Add(sign, newPunkts);
            }

            sideVariants = sideVariants
                .Where(v => v.Value.All(e => e.X <= allData.SquareSide
                    && e.X >= 0
                    && e.Y <= allData.SquareSide
                    && e.Y >= 0
                ))
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            var selectedPath = sideVariants
                .Select(kv => new { Side = kv.Key, Length = GeometryUtils.TotalLengthOfPath(kv.Value) })
                .OrderBy(a => a.Length)
                .Select(a => sideVariants[a.Side])
                .First();

            var newLegs = ReplaceLeg(legs, targetCrossedLeg, selectedPath);

            return newLegs;
        }

        private List<Leg> ReplaceLeg(List<Leg> legs, Leg targetCrossedLeg, List<IPunkt> selectedPath)
        {
            var newLegs = new List<Leg>();

            var insertLegs = new List<Leg>();

            for (int i = 0; i < selectedPath.Count - 1; i++)
            {
                var leg = new Leg(selectedPath[i], selectedPath[i + 1]);

                insertLegs.Add(leg);
            }

            foreach (var leg in legs)
            {
                if (leg != targetCrossedLeg)
                {
                    newLegs.Add(leg);
                }
                else
                {
                    newLegs.AddRange(insertLegs);
                }
            }

            return newLegs;
        }

        private void CheckCrossesWithSnowAreas(List<Leg> legs)
        {
            foreach (var leg in legs)
            {
                var crossedArea = new Dictionary<SnowArea, double>();

                foreach (var snowArea in allData.SnowAreas)
                {
                    var overlapLength = GeometryUtils.GetOverlapWithSnowArea(snowArea, leg.From, leg.To,
                        out var t1, out var t2);

                    if (overlapLength < 1e-8)
                    {
                        continue;
                    }

                    if (GeometryUtils.IsPunktInSnowArea(snowArea, leg.From))
                    {
                        leg.FromSnowArea = snowArea;
                    }

                    if (GeometryUtils.IsPunktInSnowArea(snowArea, leg.To))
                    {
                        leg.ToSnowArea = snowArea;
                    }

                    if (!GeometryUtils.IsPunktInSnowArea(snowArea, leg.From)
                        && !GeometryUtils.IsPunktInSnowArea(snowArea, leg.To)
                        )
                    {
                        crossedArea.Add(snowArea, t1);
                    }
                }

                leg.CrossSnowAreas = crossedArea
                    .OrderBy(kv => kv.Value)
                    .Select(kv => kv.Key)
                    .ToList();
            }
        }
    }
}

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

        public bool IsInSingleSnowArea => !CrossSnowAreas.Any()
            && FromSnowArea != null
            && FromSnowArea == ToSnowArea;

        public bool IsEnterToSnowArea => !CrossSnowAreas.Any()
            && (((FromSnowArea == null) && (ToSnowArea != null)) ||
                ((FromSnowArea != null) && (ToSnowArea == null)));

        public bool IsClean => FromSnowArea == null
            && ToSnowArea == null
            && !CrossSnowAreas.Any();

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

        private static readonly double ARC_SIZE = Math.PI / 64;
        private static readonly double RADIUS_EXPAND = 1.002d;
        private static readonly List<double> ANGLE_GRID = Enumerable
            .Range(0, (int)Math.Round(2 * Math.PI / ARC_SIZE))
            .Select(d => d * ARC_SIZE - Math.PI)
            .ToList();

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

                // var json = LegsToJSON(legs, route);

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

            if (legs.Count == 1 && legs.Any(leg => leg.IsInSingleSnowArea))
            {
                legs = TryFindBetterRoundRoute(legs.Single());

                //var json = LegsToJSON(legs, route);
            }

            if (legs.Any(leg => leg.IsEnterToSnowArea))
            {
                var json = LegsToJSON(legs, route);

                if (legs.First().IsEnterToSnowArea)
                {
                    legs = FindBestRouteInSnowarea(legs, legs.First());
                }

                CheckCrossesWithSnowAreas(legs);

                json = LegsToJSON(legs, route);

                if (legs.Last().IsEnterToSnowArea)
                {
                    legs = FindBestRouteInSnowarea(legs, legs.Last());
                }

                CheckCrossesWithSnowAreas(legs);

                json = LegsToJSON(legs, route);

                iteration = 0;

                while (true)
                {
                    iteration++;

                    if (iteration > 50)
                    {
                        throw new StackOverflowException();
                    }

                    // var json = LegsToJSON(legs, route);

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

                json = LegsToJSON(legs, route);
            }

            CheckCrossesWithSnowAreas(legs);

            legs = TryCompactifyLegs(legs, route);

            route.Punkts = legs
                .Select(e => e.From)
                .Append(route.To)
                .ToList();

            CheckDuplicates(route);

            CalculateDistance(route);
        }

        private List<Leg> TryCompactifyLegs(List<Leg> legs, Route route)
        {
            /// Для одного перехода сокращение невозможно
            if (legs.Count <= 1)
            {
                return legs;
            }

            int iteration = 0;

            while (true)
            {
                iteration++;

                if (iteration > 200)
                {
                    throw new StackOverflowException();
                }

                bool doRestart = false;

                for (int i = 0; i < legs.Count - 1; i++)
                {
                    var directLeg = new Leg(legs[i].From, legs[i + 1].To);

                    CheckCrossesWithSnowAreas(directLeg);

                    if (directLeg.IsClean && legs[i].IsClean && legs[i + 1].IsClean)
                    {
                        doRestart = true;

                        var newLegs = new List<Leg>();

                        for (int j = 0; j < i; j++)
                        {
                            newLegs.Add(legs[j]);
                        }

                        newLegs.Add(directLeg);

                        for (int j = i + 2; j < legs.Count; j++)
                        {
                            newLegs.Add(legs[j]);
                        }

                        legs = newLegs;
                    }

                    if (doRestart)
                    {
                        break;
                    }
                }

                if (!doRestart)
                {
                    break;
                }
            }

            return legs;
        }

        private List<Leg> FindBestRouteInSnowarea(List<Leg> legs, Leg targetLeg)
        {
            var snowArea = targetLeg.FromSnowArea ?? targetLeg.ToSnowArea;

            if (snowArea == null)
            {
                throw new Exception();
            }

            var outPoint = targetLeg.FromSnowArea != null ? targetLeg.To : targetLeg.From;
            var inPoint  = targetLeg.FromSnowArea != null ? targetLeg.From : targetLeg.To;

            var tangentAngle = Math.Acos(snowArea.R / GeometryUtils.GetDistance(snowArea, outPoint));
            var outAngle = GeometryUtils.GetPolarAngle(snowArea, outPoint);

            /// Ключ - угол, значение - время движения
            var timesToMove = ANGLE_GRID
                .ToDictionary(
                    g => g,
                    g => 0.0d
                );

            foreach (var angle in ANGLE_GRID)
            {
                var tPunke = new DerPunkt() 
                {
                    X = snowArea.X + (int)Math.Round(snowArea.R * RADIUS_EXPAND * Math.Cos(angle)),
                    Y = snowArea.Y + (int)Math.Round(snowArea.R * RADIUS_EXPAND * Math.Sin(angle)),
                    PunktType = PunktType.FREE,
                };

                if (!GeometryUtils.IsPointInArea(tPunke, allData))
                {
                    timesToMove[angle] = 1e9;
                    continue;
                }

                var innerDistance = GeometryUtils.GetDistance(inPoint, tPunke);
                /// Не нужно стоить дугу
                if (GeometryUtils.AngleDistance(outAngle, angle) <= tangentAngle)
                {
                    var outerDistance = GeometryUtils.GetDistance(outPoint, tPunke);

                    timesToMove[angle] = innerDistance / allData.SnowSpeed + outerDistance / allData.AirSpeed;
                }
                else
                {
                    var tangentAngle1 = GeometryUtils.SumPolarAngles(outAngle, +tangentAngle);
                    var tangentAngle2 = GeometryUtils.SumPolarAngles(outAngle, -tangentAngle);

                    var angleDistance1 = GeometryUtils.AngleDistance(tangentAngle1, angle);
                    var angleDistance2 = GeometryUtils.AngleDistance(tangentAngle2, angle);

                    var minAngleDistance = Math.Min(angleDistance1, angleDistance2);

                    var outerDistance = Math.Tan(tangentAngle) * snowArea.R * RADIUS_EXPAND +
                        snowArea.R * RADIUS_EXPAND * minAngleDistance;

                    timesToMove[angle] = innerDistance / allData.SnowSpeed + outerDistance / allData.AirSpeed;
                }
            }

            var minTravelTime = timesToMove.Min(kv => kv.Value);

            var entryAngle = timesToMove
                .Where(kv => kv.Value == minTravelTime)
                .First()
                .Key;

            var entryPunkt = new DerPunkt()
            {
                X = snowArea.X + (int)Math.Round(snowArea.R * RADIUS_EXPAND * Math.Cos(entryAngle)),
                Y = snowArea.Y + (int)Math.Round(snowArea.R * RADIUS_EXPAND * Math.Sin(entryAngle)),
                PunktType = PunktType.FREE,
            };

            var punkts = new List<IPunkt>()
            {
                outPoint,
            };

            /// Не нужно стоить дугу, тупо соединяем через внутреннюю точку на окружности
            if (GeometryUtils.AngleDistance(outAngle, entryAngle) <= tangentAngle)
            {
                punkts.Add(entryPunkt);
            }
            else
            {
                var tangentAngle1 = GeometryUtils.SumPolarAngles(outAngle, +tangentAngle);
                var tangentAngle2 = GeometryUtils.SumPolarAngles(outAngle, -tangentAngle);

                var angleDistance1 = GeometryUtils.AngleDistance(tangentAngle1, entryAngle);
                var angleDistance2 = GeometryUtils.AngleDistance(tangentAngle2, entryAngle);

                var arcPunkts = new List<DerPunkt>();

                if (angleDistance1 < angleDistance2)
                {
                    arcPunkts = MakeArcAroundSnowArea(snowArea, tangentAngle1, entryAngle);
                }
                else
                {
                    arcPunkts = MakeArcAroundSnowArea(snowArea, tangentAngle2, entryAngle);
                }

                punkts.AddRange(arcPunkts);
            }

            punkts.Add(inPoint);

            /// Если что-то вылезло за край, то ничего не делаем
            if (punkts.Any(e => !GeometryUtils.IsPointInArea(e, allData)))
            {
                return legs;
            }

            /// Если старт в снежной области, то обращаем пункты, так как строим от внешнего
            if (targetLeg.FromSnowArea != null)
            {
                punkts.Reverse();
            }

            var newLegs = new List<Leg>();

            for (int i = 0; i < punkts.Count - 1; i++)
            {
                newLegs.Add(new Leg(punkts[i], punkts[i + 1]));
            }

            return ReplaceLeg(legs, targetLeg, punkts);
        }

        private static string LegsToJSON(List<Leg> legs, Route route)
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

            return JsonConvert.SerializeObject(tunkts);
        }

        private List<Leg> TryFindBetterRoundRoute(Leg leg)
        {
            var snowArea = leg.FromSnowArea;

            if (snowArea == null)
            {
                throw new Exception();
            }

            var directThroughSnowDistance = GeometryUtils.GetDistance(leg.From, leg.To);
            var directThroughSnowTime = directThroughSnowDistance / allData.SnowSpeed;

            var phiFrom = GeometryUtils.GetPolarAngle(snowArea, leg.From);
            var phiTo = GeometryUtils.GetPolarAngle(snowArea, leg.To);

            var dphi = GeometryUtils.AngleDistance(phiFrom, phiTo);

            var distanceFrom2CircleBoundary = snowArea.R - GeometryUtils.GetDistance(snowArea, leg.From);
            var distanceTo2CircleBoundary = snowArea.R - GeometryUtils.GetDistance(snowArea, leg.To);

            var outDistance = (RADIUS_EXPAND - 1.0d) * snowArea.R * 2;
            var arcDistance = RADIUS_EXPAND * snowArea.R * dphi;

            var aroundSnowTime = (distanceFrom2CircleBoundary + distanceTo2CircleBoundary) / allData.SnowSpeed
                + (outDistance + arcDistance) / allData.AirSpeed;

            /// Если в обход длиннее, то ничего не меняем
            if (aroundSnowTime >= directThroughSnowTime)
            {
                return new List<Leg>() { leg };
            }

            List<DerPunkt> arcPunkts = MakeArcAroundSnowArea(snowArea, phiFrom, phiTo);

            var newPunkts = new List<IPunkt>() { leg.From }
                .Concat(arcPunkts)
                .Append(leg.To)
                .ToList();

            if (newPunkts.Any(p => 
                    p.X < 0 || p.X > allData.SquareSide
                ||  p.Y < 0 || p.Y > allData.SquareSide
            ))
            {
                return new List<Leg>() { leg };
            }

            var newLegs = new List<Leg>();

            for (int i = 0; i < newPunkts.Count - 1; i++)
            {
                newLegs.Add(new Leg(newPunkts[i], newPunkts[i + 1]));
            }

            return newLegs;
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
                var fromAngle = GeometryUtils.SumPolarAngles(angleFrom, sign * tangentAngleFrom);
                var toAngle = GeometryUtils.SumPolarAngles(angleTo, -sign * tangentAngleTo);

                List<DerPunkt> arcPunkts = MakeArcAroundSnowArea(snowArea, fromAngle, toAngle);

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

        private static List<DerPunkt> MakeArcAroundSnowArea(SnowArea snowArea, double fromAngle, double toAngle)
        {
            List<double> arcNodes = new();

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

            return arcPunkts;
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

        private void CheckCrossesWithSnowAreas(Leg leg)
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

        private void CheckCrossesWithSnowAreas(List<Leg> legs)
        {
            foreach (var leg in legs)
            {
                CheckCrossesWithSnowAreas(leg);
            }
        }
    }
}

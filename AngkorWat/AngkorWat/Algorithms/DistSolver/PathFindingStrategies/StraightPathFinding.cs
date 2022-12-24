﻿using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.DistSolver.PathFindingStrategies
{
    internal class StraightPathFinding : IPathFindingStrategy
    {
        protected AllData allData;
        public StraightPathFinding(AllData allData) 
        {
            this.allData = allData;
        }
        public void Calculate(Route route)
        {
            route.TravelTime = 0.0d;

            var totalLenght = GeometryUtils.GetDistance(route.From, route.To);
            var snowLength = 0.0d;

            foreach (var snowArea in allData.SnowAreas)
            {
                snowLength += GeometryUtils.GetOverlapWithSnowArea(snowArea, route.From, route.To);
            }

            var cleanLength = totalLenght - snowLength;

            route.TravelTime = cleanLength / allData.AirSpeed + snowLength / allData.SnowSpeed;
            route.Distance = totalLenght;
        }
    }
}
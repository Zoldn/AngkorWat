using AngkorWat.Algorithms.DistSolver.PathFindingStrategies;
using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.DistSolver
{
    internal interface IPathFindingStrategy
    {
        /// <summary>
        /// Метод должен заполнить поля Punkts,
        /// первая и посленяя точка которых должны быть route.From и route.To соответственно.
        /// Также должны быть заполнены поля TravelTime и Distance
        /// </summary>
        /// <param name="route"></param>
        public void Calculate(Route route);
    }

    internal class DistanceSolver
    {
        private readonly AllData allData;

        public DistanceSolver(AllData allData)
        {
            this.allData = allData;
        }

        public DistanceSolution Solve()
        {
            var solution = new DistanceSolution();

            var locations = allData.Children
                .Select(c => c as ILocation)
                .Append(allData.Santa);

            IPathFindingStrategy pathFindingStrategy = new StraightPathFinding(allData);

            foreach (var from in locations)
            {
                foreach (var to in locations)
                {
                    if (from.LocationId >= to.LocationId)
                    {
                        continue;
                    }

                    var route = new Route(from, to);


                    pathFindingStrategy.Calculate(route);
                    solution.Routes.Add((from, to), route);

                    solution.Routes.Add((to, from), route.AsReverse());
                }
            }

            return solution;
        }
    }
}

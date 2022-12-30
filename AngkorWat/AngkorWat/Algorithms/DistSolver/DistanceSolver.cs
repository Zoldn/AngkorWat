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
        private readonly Data allData;
        public HashSet<(int, int)> ChildrenPositions { get; private set; }

        public DistanceSolver(Data allData)
        {
            this.allData = allData;

            ChildrenPositions = new HashSet<(int, int)>();
        }

        public DistanceSolution Solve()
        {
            var solution = new DistanceSolution();

            SetupChildrenPositions();

            var locations = allData.Children
                .Select(c => c as ILocation)
                .Append(allData.Santa);

            //IPathFindingStrategy pathFindingStrategy = new StraightPathFinding(allData);
            IPathFindingStrategy pathFindingStrategy = new GreedPathFinding(allData);

            foreach (var from in locations)
            {
                foreach (var to in locations)
                {
                    if (from.Id >= to.Id)
                    {
                        continue;
                    }

                    var route = new Route(from, to);


                    pathFindingStrategy.Calculate(route);

                    CheckAdditionalPointNotChildren(route);

                    solution.Routes.Add((from, to), route);

                    solution.Routes.Add((to, from), route.AsReverse());
                }
            }

            return solution;
        }

        private void SetupChildrenPositions()
        {
            ChildrenPositions = allData
                .Children
                .Select(e => (e.X, e.Y))
                .ToHashSet();
        }

        private void CheckAdditionalPointNotChildren(Route route)
        {
            for (int i = 0; i < route.Punkts.Count; i++)
            {
                var punkt = route.Punkts[i];

                if (punkt.PunktType == PunktType.CHILD 
                    || punkt.PunktType == PunktType.SANTA
                    )
                {
                    continue;
                }

                if (!ChildrenPositions.Contains((punkt.X, punkt.Y)))
                {
                    continue;
                }

                int x = punkt.X;

                while (ChildrenPositions.Contains((x, punkt.Y)))
                {
                    x += 1;
                }

                route.Punkts[i] = new DerPunkt()
                {
                    X = x,
                    Y = punkt.Y,
                    PunktType = punkt.PunktType
                };
            }
        }
    }
}

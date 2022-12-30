using AngkorWat.Algorithms.DistSolver;
using AngkorWat.Algorithms.PackSolver;
using AngkorWat.Algorithms.RouteSolver;
using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    internal class Data
    {
        /// <summary>
        /// Id карты
        /// </summary>
        public string MapId { get; init; }
        /// <summary>
        /// Максимальная стоимость подарков
        /// </summary>
        public int MaxGiftCost { get; init; }
        /// <summary>
        /// Скорость в воздухе [км/с]
        /// </summary>
        public int AirSpeed => 70;
        /// <summary>
        /// Скорость в снегу [км/с]
        /// </summary>
        public int SnowSpeed => 10;
        /// <summary>
        /// Локация базы Санты
        /// </summary>
        public Santa Santa { get; }
        /// <summary>
        /// Размер квадрата с нижней левой вершиной в (0, 0), который нельзя покидать
        /// </summary>
        public int SquareSide => 10000;
        /// <summary>
        /// Максимальная масса подарков в санях
        /// </summary>
        public int WeightLimit => 200;
        /// <summary>
        /// Максимальный объем подарков в санях
        /// </summary>
        public int VolumeLimit => 100;

        public List<Phase1Child> Children { get; set; }
        public List<SnowArea> SnowAreas { get; set; }
        public List<Gift> Gifts { get; set; }
        
        public Data()
        {
            Santa = new();

            Children = new();
            SnowAreas = new();
            Gifts = new();
            MapId = "";
        }
    }
}

using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    /// <summary>
    /// Внутреннее представление входных данных
    /// </summary>
    internal class Data
    {
        /// <summary>
        /// Id карты
        /// </summary>
        public string MapId { get; init; }
        
        
        public Data()
        {
            MapId = string.Empty;
        }
    }
}

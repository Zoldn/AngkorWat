using AngkorWat.IO;
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
        public List<Active> Actives { get; set; } = new();
        public List<New> News { get; internal set; } = new();
        public List<AggregatedNew> AggregatedNews { get; internal set; } = new();

        public AccountInfo AccountInfo { get; internal set; } = new();

        public Data()
        {
            
        }
    }
}

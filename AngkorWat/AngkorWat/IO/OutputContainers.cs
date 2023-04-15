using AngkorWat.Components;
using AngkorWat.IO.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.IO
{
    internal class Phase1OutputContainer : BaseOutputContainer
    {
        public Phase1OutputContainer(Data data)
            : base(data.MapId)
        {
            
        }
    }
}

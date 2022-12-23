using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.IO
{
    internal class InputContainer
    {
        public List<RawGift> gifts { get; set; }
        public List<RawSnowArea> snowAreas { get; set; }
        public List<RawChild> children { get; set; }
        public InputContainer()
        {
            gifts = new();
            snowAreas = new();
            children = new();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.IO
{
    internal class InputContainer
    {
        public Dictionary<string, Dictionary<string, string>> Input { get; set; }
        public InputContainer()
        {
            Input = new();
        }
    }
}

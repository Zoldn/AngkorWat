using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.IO
{
    internal class Phase1InputContainer
    {
        public List<RawPhase1Gift> gifts { get; set; }
        public List<RawSnowArea> snowAreas { get; set; }
        public List<RawPhase1Child> children { get; set; }
        public Phase1InputContainer()
        {
            gifts = new();
            snowAreas = new();
            children = new();
        }
    }

    internal class Phase2InputContainer
    {
        public List<RawPhase2Gift> gifts { get; set; }
        public List<RawPhase2Child> children { get; set; }
        public Phase2InputContainer()
        {
            gifts = new();
            children = new();
        }
    }

    internal class Phase3InputContainer
    {
        public List<RawSnowArea> snowAreas { get; set; }
        public List<RawPhase3Gift> gifts { get; set; }
        public List<RawPhase3Child> children { get; set; }
        public Phase3InputContainer()
        {
            gifts = new();
            children = new();
            snowAreas = new();
        }
    }
}

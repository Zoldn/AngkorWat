using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.IO
{
    internal class RawPhase1Gift
    {
        public int id { get; set; }
        public int weight { get; set; }
        public int volume { get; set; }
        public override string ToString()
        {
            return $"Gift {id}, {weight}/{volume}";
        }
    }

    internal class RawSnowArea
    {
        public int x { get; set; }
        public int y { get; set; }
        public int r { get; set; }
        public override string ToString()
        {
            return $"SnowArea in ({x}, {y}) with {r}";
        }
    }

    internal class RawPhase1Child
    {
        public int x { get; set; }
        public int y { get; set; }
        public override string ToString()
        {
            return $"Child in ({x}, {y})";
        }
    }

    internal class RawPhase2Gift
    {
        public int id { get; set; }
        public string type { get; set; }
        public int price { get; set; }
    }

    internal class RawPhase2Child
    {
        public int id { get; set; }
        public string gender { get; set; }
        public int age { get; set; }
    }


    internal class RawPhase3Gift
    {
        public int id { get; set; }
        public int weight { get; set; }
        public int volume { get; set; }
        public string type { get; set; }
        public int price { get; set; }
        public override string ToString()
        {
            return $"Gift {id}, {weight}/{volume}/{type}/{price}";
        }
    }

    internal class RawPhase3Child
    {
        public int x { get; set; }
        public int y { get; set; }
        public int id { get; set; }
        public string gender { get; set; }
        public int age { get; set; }
        public override string ToString()
        {
            return $"Child {id} in ({x}, {y})/{gender}/{age}";
        }
    }
}

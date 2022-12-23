using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.IO
{
    internal class RawGift
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

    internal class RawChild
    {
        public int x { get; set; }
        public int y { get; set; }
        public override string ToString()
        {
            return $"Child in ({x}, {y})";
        }
    }
}

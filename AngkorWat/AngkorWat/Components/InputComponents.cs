using AngkorWat.Algorithms.DistSolver;
using AngkorWat.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    internal class Gift
    {
        public int Id { get; set;  }
        public int Weight { get; set; }
        public int Volume { get; set; }

        public Gift()
        {
        }
        public Gift(RawGift rawGift)
        {
            Id = rawGift.id;
            Weight = rawGift.weight;
            Volume = rawGift.volume;
        }
        public override string ToString()
        {
            return $"Gift {Id}, {Weight}/{Volume}";
        }
    }

    internal class SnowArea : IPunkt
    {
        public int X { get; }
        public int Y { get; }
        public int R { get; }
        public PunktType PunktType => PunktType.SNOWAREA;
        public SnowArea(RawSnowArea rawSnowArea)
        {
            X = rawSnowArea.x;
            Y = rawSnowArea.y;
            R = rawSnowArea.r;
        }
        public override string ToString()
        {
            return $"SnowArea in ({X}, {Y}) with {R}";
        }
    }

    internal class Child : ILocation
    {
        public int X { get; }
        public int Y { get; }
        public bool IsSanta => false;
        public PunktType PunktType => PunktType.CHILD;
        public int LocationId { get; set; }
        public Child(RawChild rawChild, int id)
        {
            X = rawChild.x;
            Y = rawChild.y;
            LocationId = id;
        }

        public DerPunkt AsPunkt()
        {
            return new DerPunkt { X = X, Y = Y, PunktType = PunktType };
        }

        public override string ToString()
        {
            return $"Child in ({X}, {Y})";
        }
    }



    internal class Phase2Gift
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int Price { get; set; }

        public Phase2Gift(RawPhase2Gift rawGift)
        {
            Id = rawGift.id;
            Type = rawGift.type;
            Price = rawGift.price;
        }

        public override string ToString()
        {
            return $"Gift {Id}, {Type}/{Price}";
        }
    }

    internal class Phase2Child
    {
        public int Id { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }

        public Phase2Child(RawPhase2Child rawChild)
        {
            Id = rawChild.id;
            Gender = rawChild.gender;
            Age = rawChild.age;
        }

        public override string ToString()
        {
            return $"Child {Id}, {Gender}/{Age}";
        }
    }
}

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
        public int Id { get; }
        public int Weight { get; }
        public int Volume { get; }
        public string Type { get; }
        public int Price { get; }

        public Gift(RawPhase1Gift rawGift)
        {
            Id = rawGift.id;
            Weight = rawGift.weight;
            Volume = rawGift.volume;
            Type = "";
            Price = 0;
        }

        public Gift(RawPhase2Gift rawGift)
        {
            Id = rawGift.id;
            Type = rawGift.type;
            Price = rawGift.price;
        }

        public Gift(RawPhase3Gift rawGift)
        {
            Id = rawGift.id;
            Weight = rawGift.weight;
            Volume = rawGift.volume;
            Type = rawGift.type;
            Price = rawGift.price;
        }

        public override string ToString()
        {
            return $"Gift {Id}, (W: {Weight}/V: {Volume}) {Type}, {Price}";
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
        public string Gender { get; }
        public int Age { get; }
        public bool IsSanta => false;
        public PunktType PunktType => PunktType.CHILD;
        public int Id { get; set; }
        public Child(RawPhase1Child rawChild, int id)
        {
            X = rawChild.x;
            Y = rawChild.y;
            Id = id;
            Gender = "";
            Age = 0;
        }

        public Child(RawPhase2Child rawChild)
        {
            X = 0;
            Y = 0;
            Id = rawChild.id;
            Gender = rawChild.gender;
            Age = rawChild.age;
        }

        public Child(RawPhase3Child rawChild, int id)
        {
            X = rawChild.x;
            Y = rawChild.y;
            Id = id;
            Gender = rawChild.gender;
            Age = rawChild.age;
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
}

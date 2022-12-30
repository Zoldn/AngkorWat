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

    internal class Phase1Child : ILocation
    {
        public int X { get; }
        public int Y { get; }
        public string Gender { get; }
        public int Age { get; }
        public bool IsSanta => false;
        public PunktType PunktType => PunktType.CHILD;
        public int Id { get; set; }
        public Phase1Child(RawPhase1Child rawChild, int id)
        {
            X = rawChild.x;
            Y = rawChild.y;
            Id = id;
            Gender = "";
            Age = 0;
        }

        public Phase1Child(RawPhase2Child rawChild)
        {
            X = 0;
            Y = 0;
            Id = rawChild.id;
            Gender = rawChild.gender;
            Age = rawChild.age;
        }

        public Phase1Child(RawPhase3Child rawChild, int id)
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


    /*
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

        public Phase2Gift(Phase3Gift g)
        {
            Id = g.Id;
            Type = g.Type;
            Price = g.Price;
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

        public Phase2Child(Phase3Child c)
        {
            Id = c.Id;
            Gender = c.Gender;
            Age = c.Age;
        }

        public override string ToString()
        {
            return $"Child {Id}, {Gender}/{Age}";
        }
    }

    internal class Phase3Child : IPhase1Child
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Id { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public bool IsSanta => false;
        public PunktType PunktType => PunktType.CHILD;
        public int Id { get; set ; }
        public Phase3Child(RawPhase3Child rawChild, int index)
        {
            Id = index;
            Gender = rawChild.gender;
            Age = rawChild.age;
            X = rawChild.x;
            Y = rawChild.y;

            Id = rawChild.id;
        }

        public DerPunkt AsPunkt()
        {
            return new DerPunkt { X = X, Y = Y, PunktType = PunktType };
        }

        public override string ToString()
        {
            return $"Child {Id}, {Gender}/{Age} in ({X}, {Y})";
        }
    }

    internal class Phase3Gift
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int Price { get; set; }
        public int Weight { get; set; }
        public int Volume { get; set; }

        public Phase3Gift(RawPhase3Gift rawGift)
        {
            Id = rawGift.id;
            Type = rawGift.type;
            Price = rawGift.price;
            Weight = rawGift.weight;
            Volume = rawGift.volume;
        }

        public override string ToString()
        {
            return $"Gift {Id}, {Type}/{Price}, ({Weight}/{Volume})";
        }
    }
    */
}

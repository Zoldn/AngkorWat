﻿using AngkorWat.IO;
using System;
using System.Collections.Generic;
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

    internal class SnowArea
    {
        public int X { get; }
        public int Y { get; }
        public int R { get; }
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

    internal class Child
    {
        public int X { get; }
        public int Y { get; }
        public Child(RawChild rawChild)
        {
            X = rawChild.x;
            Y = rawChild.y;
        }
        public override string ToString()
        {
            return $"Child in ({X}, {Y})";
        }
    }
}
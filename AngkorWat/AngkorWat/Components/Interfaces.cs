using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    public interface IShootStrategy
    {
        public void AddCommand(WorldState worldState);
    }

    public interface IBuildStrategy
    {
        public void AddCommand(WorldState worldState);
    }

    public interface IMoveCenterStrategy
    {
        public void AddCommand(WorldState worldState);
    }
}

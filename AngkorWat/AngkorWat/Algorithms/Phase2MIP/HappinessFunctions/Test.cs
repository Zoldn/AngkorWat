using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.Phase2MIP.HappinessFunctions
{
    internal class TestHappinessFunction : IHappinessFunction
    {
        public int GetHappiness(ChildrenGroup childrenGroup, GiftGroup giftGroup)
        {
            return giftGroup.Price / 10;
        }
    }
}

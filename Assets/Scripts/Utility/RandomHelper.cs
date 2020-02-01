using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utility
{
    public static class RandomHelper
    {
        public static int NextExcluding(this System.Random random, int min, int max, int exlcude)
        {
            int rnd = random.Next(min, max);
            if (rnd == exlcude)
            {
                if (rnd == max)
                    return min;
                if (rnd == min)
                    return max;
                return rnd + 1;
            }
            else
            {
                return rnd;
            }
        }
    }
}
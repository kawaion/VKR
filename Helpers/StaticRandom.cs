using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VKR.Helpers
{
    static class StaticRandom
    {
        //static int seed = Environment.TickCount;
        public static int seed=130;//128

        static readonly ThreadLocal<Random> random =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        public static int RandInt(int lowerBound, int upperBound)
        {
            //seed = InputParams.seed;
            return random.Value.Next(lowerBound, upperBound);

        }
        public static double RandDouble(double lowerBound, double upperBound)
        {
            //seed = InputParams.seed;
            return random.Value.NextDouble() * (upperBound - lowerBound) + lowerBound;
        }
    }
}

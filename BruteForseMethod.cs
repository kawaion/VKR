using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VKR.Helpers;
using System.Threading;
using System.Diagnostics;

namespace VKR
{
    class BruteForseMethod
    {
        public delegate double DelFunctionOptimized(List<double> vars);
        private CheckPassValues.DelFunctionOptimized _functionOptimized;


        public BruteForseMethod(CheckPassValues.DelFunctionOptimized functionOptimized)
        {
            _functionOptimized = functionOptimized;
        }
        public(double,double,List<List<double>>) Start(int countStep)
        {
            CheckPassValues checker = new CheckPassValues(_functionOptimized, 15);
            int iter = 0;
            bool goodValue;
            List<List<double>> badSets=new List<List<double>>();
            double V=0;
            double optimV=0;
            double optimI=0;
            double optimJ=0;
            double step = 1.0 / countStep;
            for(double i=0;i<=1;i+=step)
                for (double j = 0; j<=1; j += step)
                {
                    goodValue = checker.IsPassValue(new List<double> { i, j }, out V);
                    if (goodValue)
                    {
                        if(V>optimV)
                        {
                        optimV = V;
                        optimI = i;
                        optimJ = j;
                        }
                    }
                    else
                    {
                        badSets.Add(new List<double> { i, j });
                    }
                    
                    iter++;
                    Console.WriteLine(iter);
                }
            return (optimI, optimJ, badSets);
        }
        
    }
}

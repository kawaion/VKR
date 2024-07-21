using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKR.Helpers
{
    class FinderMaxP
    {
        public FinderMaxP(List<List<double>> solRunge,BallisticFunctions func)
        {
            _solRunge = solRunge;
            _func = func;
            f = new ConvenientVarsCall(solRunge);//не влияет на работу, просто удобство чтоб обращаться к элементам массива по имени а не по индексу
        }

        private List<List<double>> _solRunge;
        private BallisticFunctions _func;
        ConvenientVarsCall f;

        private delegate double DelFunction(double psi1,double psi2, double xt, double vsn, double lsn);
        DelFunction func;

        
        public double FindMaxPkn()
        {
            return IterateValues(_solRunge, _func.Pkn);
        }
        public double FindMaxP()
        {
            return IterateValues(_solRunge, _func.P);
        }
        public double FindMaxPsn()
        {
            return IterateValues(_solRunge, _func.Psn);
        }
        private double IterateValues(List<List<double>> massive,DelFunction func)
        {
            double MaxP = 0;
            double currentP;
            for (int i = 0; i < massive.Count; i++)
            {
                currentP = func(f.psi1(i), f.psi2(i), f.xt(i), f.vsn(i), f.lsn(i));
                if (currentP > MaxP) MaxP = currentP;
            }
            return MaxP;
        }
        public int FindIPAtMomentMax(double MaxP,double eps)
        {
            int iLimit = _solRunge.Count-1;
            for (int i = 0; i < _solRunge.Count; i++)
            {
                if (_func.Pkn(f.psi1(i),f.psi2(i), f.xt(i), f.vsn(i), f.lsn(i)) > MaxP*(eps+1))
                {
                    iLimit = i;
                    break;
                }
            }
            return iLimit;
        }
    }
}

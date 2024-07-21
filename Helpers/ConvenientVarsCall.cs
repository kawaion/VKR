using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKR.Helpers
{
    class ConvenientVarsCall
    {
        private List<List<double>> _massive;

        public ConvenientVarsCall(List<List<double>> massive)
        {
            _massive = massive;
        }

        public double t(int i) => GetMassiveValue(i, 0);
        public double z(int i) => GetMassiveValue(i, 1);
        public double psi1(int i) => GetMassiveValue(i, 2);
        public double psi2(int i) => GetMassiveValue(i, 3);
        public double xt(int i) => GetMassiveValue(i, 4);
        public double vsn(int i) => GetMassiveValue(i, 5);
        public double lsn(int i) => GetMassiveValue(i, 6);
        private double GetMassiveValue(int i, int concrectVar)
        {
             return _massive[i][concrectVar];
        }
    }
}

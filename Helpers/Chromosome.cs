using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKR.Helpers
{
    public class Chromosome : ICloneable
    {
        public double U12 { get; set; }
        public double _V { get; set; }
        public double U11 { get; set; }

        private bool mutated=false;

        public void Mutation()
        {
            mutated = true;
        }
        public bool IsMutated()
        {
            bool tmpmutated = mutated;
            mutated = false;
            return tmpmutated;
        }
        public Chromosome(List<double> gens, double? V = null)
        {
            U11 = gens[0];
            U12 = gens[1];
            if (V != null)
            _V = (double)V;
        }
        public List<double> GetParams()
        {
            return new List<double> { U11, U12 };
        }
        public double FCh
        {
            set {_V = value;}
            get {return _V;}
        }
        public void ChangeVar(int i, double value)
        {
            Mutation();
            switch (i)
            {
                case 0: { U12 = value; return; };
                case 1: { U11 = value; return; };
            }
        }
        public object Clone()
        {
            return MemberwiseClone();
        }
        public bool IsEquals(Chromosome ch2)
        {
            return U11 == ch2.U11 & U12 == ch2.U12;
        }
    }
}

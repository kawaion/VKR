using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VKR.Helpers;

namespace VKR
{
    class SolvedChromosomes
    {
        List<Chromosome> solutions=new List<Chromosome>();

        public SolvedChromosomes()
        {

        }
        public Chromosome GetReadySol(Chromosome ch)
        {
            foreach(var sol in solutions)
                if (ch.IsEquals(sol))
                    ch.FCh = sol.FCh;
            return ch;
        }
        public void AddInSol(Chromosome ch)
        {
            bool flagNewSol = true;
            foreach (var sol in solutions)
            {
                if (ch.IsEquals(sol))
                    flagNewSol = false;
                
            }
            if (ch.FCh != 0 & flagNewSol)
                solutions.Add(ch);
        }
    }
}

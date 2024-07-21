using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VKR.Helpers;

namespace VKR
{
    class GeneticAlgorithm
    {
        //public delegate double DelFunctionOptimized(List<double> vars);
        CheckPassValues.DelFunctionOptimized _functionOptimized;
        CheckPassValues checker;
        InputParams _ip;
        List<Chromosome> badChromosomes=new List<Chromosome>();
        SolvedChromosomes readySols = new SolvedChromosomes();

        //Form1 form = new Form1();
        //List<double> vars;
        int _N;
        //int NChromosome = 100;
        int NChromosome => _ip.Nchromosomes;
        Random rnd;
        public GeneticAlgorithm(CheckPassValues.DelFunctionOptimized functionOptimized, int N, InputParams ip)
        {
            _ip = ip;
            _functionOptimized = functionOptimized;
            _N = N;
            Results = new List<Chromosome>();
            StaticRandom.seed = ip.seed;
            rnd = new Random(ip.seed);
            checker = new CheckPassValues(_functionOptimized, 20);
        }

        int n;
        List<Chromosome> Results;

        public List<Chromosome> GenerateChromosomes()
        {
            var chromosomes = new List<Chromosome>();
            
            for (int i = 0; i < NChromosome; i++)
                chromosomes.Add(new Chromosome(GetRandGens()));
            bool goodValue;
            int place = NChromosome;
            
            do
            {
                leader = chromosomes[place-1];
                //
            _functionOptimized(leader.GetParams());
                //
                double tmpFCh;
                goodValue = checker.IsPassValue(leader.GetParams(),out tmpFCh);
                leader.FCh = tmpFCh;
                place--;
            } while(!goodValue);
            Results.Add((Chromosome)leader.Clone());
            return chromosomes;
        }
        private List<double> GetRandGens()
        {
            var gens = new List<double>(_N);
            for (int j = 0; j < _N; j++)
                gens.Add(rnd.NextDouble());
            return gens;
        }
        double max = 0;
        Chromosome leader;
        //double crossOverChance = 1;
        //double mutationChance = 0.01;
        double crossOverChance => _ip.crossOverChance;
        double mutationChance => _ip.mutationChance;
        private List<T> MyConcat<T>(List<T> x, List<T> y)
        {
            for (int i = 0; i < y.Count; i++)
                x.Add(y[i]);
            return x;
        }
        public List<Chromosome> CrossOver(List<Chromosome> chromosomes)
        {
            int NC = chromosomes.Count;
            List<Chromosome> newChromosomes = new List<Chromosome>();
            Chromosome ch1;
            Chromosome ch2;
            int ich;
            List<double> ch1H;
            List<double> ch2H;
            List<double> ch1T;
            List<double> ch2T;
            for (int i = NC - 1; i > 0; i -= 2)
            {
                ich = StaticRandom.RandInt(0, i);
                ch1 = chromosomes[ich];
                chromosomes.Remove(ch1);
                ich = StaticRandom.RandInt(0, i - 1);
                ch2 = chromosomes[ich];
                chromosomes.Remove(ch2);
                if (StaticRandom.RandDouble(0, 1) < crossOverChance)
                {
                    int divisionPointChr = StaticRandom.RandInt(1, _N);
                    ch1H = ch1.GetParams().GetRange(0, divisionPointChr);
                    ch2H = ch2.GetParams().GetRange(0, divisionPointChr);
                    ch1T = ch1.GetParams().GetRange(divisionPointChr, _N - divisionPointChr);
                    ch2T = ch2.GetParams().GetRange(divisionPointChr, _N - divisionPointChr);
                    newChromosomes.Add(ch1);
                    newChromosomes.Add(ch2);
                    newChromosomes.Add(new Chromosome(MyConcat<double>(ch1H, ch2T)));
                    newChromosomes.Add(new Chromosome(MyConcat<double>(ch2H, ch1T)));
                }
            }
            if(chromosomes.Count==1) newChromosomes.Add(chromosomes[0]);
            return newChromosomes;

        }
        public List<Chromosome> Mutation(List<Chromosome> chromosomes)
        {
            foreach (var el in chromosomes)
            {
                if (StaticRandom.RandDouble(0, 1) < mutationChance)
                {
                    int i = StaticRandom.RandInt(0, _N - 1);
                    el.ChangeVar(i, rnd.NextDouble());
                }
            }
            return chromosomes;
        }
        public List<Chromosome> Selection(List<Chromosome> chromosomes)
        {
            Console.WriteLine("селекция:");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            int NC = chromosomes.Count;
            List<Chromosome> newChromosomes = new List<Chromosome>();
            Chromosome ch1;
            Chromosome ch2;
            int ich;
            for (int i = NC - 1; i > 0; i -= 2)
            {

                ich = StaticRandom.RandInt(0, i);
                ch1 = chromosomes[ich];
                chromosomes.Remove(ch1);
                ich = StaticRandom.RandInt(0, i - 1);
                ch2 = chromosomes[ich];
                chromosomes.Remove(ch2);
                bool goodValue1 = true, goodValue2 = true;
                ch1 = readySols.GetReadySol(ch1);
                ch2 = readySols.GetReadySol(ch2);
                if (ch1.FCh == 0 || ch1.IsMutated())
                {
                    double tmpFCh;
                    goodValue1 = checker.IsPassValue(ch1.GetParams(), out tmpFCh);
                    ch1.FCh = tmpFCh;
                }
                    
                if (ch2.FCh == 0 || ch2.IsMutated())
                {
                    double tmpFCh;
                    goodValue2 = checker.IsPassValue(ch2.GetParams(), out tmpFCh);
                    ch2.FCh = tmpFCh;
                }
                if (ch1.FCh > ch2.FCh)
                {
                    newChromosomes.Add(ch1);
                    readySols.AddInSol(ch1);
                    if (leader.FCh < ch1.FCh)
                        leader = (Chromosome)ch1.Clone();
                    if (!goodValue2) badChromosomes.Add(ch2);
                }
                else
                {
                    if (!goodValue2) badChromosomes.Add(ch2);
                    else
                    {
                        newChromosomes.Add(ch2);
                        readySols.AddInSol(ch2);
                        if (leader.FCh < ch2.FCh)
                            leader = (Chromosome)ch2.Clone();
                    }
                    if (!goodValue1) badChromosomes.Add(ch1);
                }
                Console.WriteLine("{0:00}:{1:00}:{2:00}",
            stopWatch.Elapsed.Hours, stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds);
            }
            stopWatch.Stop();
            Results.Add((Chromosome)leader.Clone());
            return newChromosomes;
            //newChromosomes.Add((Chromosome)leader.Clone());
            //test.Add(newChromosomes);
        }
        public List<Chromosome> GetBadChromosomes()
        {
            return badChromosomes;
        }
        private List<Chromosome> AddNewChromosomesIfLessHalfN(List<Chromosome> ch)
        {
            while(ch.Count< NChromosome/2)
            {
                ch.Add(new Chromosome(GetRandGens()));
            }
            return ch;
        }
        double counterEps => _ip.counterEps;
        double eps => _ip.epsGA;
        public (Chromosome, List<Chromosome>) Start()
        {
            List<Chromosome> chromosomes;
            int count = 0;
            chromosomes = GenerateChromosomes();
            int iter = 0;
            while (count < counterEps)
            {
                chromosomes = Selection(chromosomes);
                chromosomes = AddNewChromosomesIfLessHalfN(chromosomes);
                chromosomes = CrossOver(chromosomes);
                chromosomes = Mutation(chromosomes);

                var smth = Math.Abs(Results[iter].FCh - Results[iter + 1].FCh);
                if (smth <= eps)
                {
                    count++;
                }
                else
                {
                    count = 0;
                }
                iter++;
                //form.WriteIter(iter.ToString());
                Console.WriteLine(iter);
            }
            return (Results.Last(), Results);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VKR.Helpers;

namespace VKR
{
    class RungeKutta
    {
        public RungeKutta(DelPrecisionCond condp, DelStopingCondition conds, List<double> init_values, List<DelFunctions> func, BallisticFunctions fun)
        {
            ip = fun._ip;
            Count_y = init_values.Count;
            StopingCondition = conds;
            PrecisionCond = condp;
            functions = func;
            this.fun = fun;
            this.init_values = init_values;
            K = new double[5, Count_y];
        }
        InputParams ip;
        int Count_y;
        public static double Tau;
        const int pRungeKutta = 4;
        double[,] K;
        //Stack<double> Y = new Stack<double>();
        List<List<double>> massive = new List<List<double>>();
        List<double> init_values;
        private List<double> parameters;
        string pathexcel = Path.GetFullPath(@"testrunge.xlsx");
        private Queue<double> Y;

        public delegate bool DelStopingCondition(List<double> vars);
        public delegate bool DelPrecisionCond(List<double> y1, List<double> y2, int p);
        public delegate double DelFunctions(List<double> vars);
        DelStopingCondition StopingCondition;
        DelPrecisionCond PrecisionCond;
        List<DelFunctions> functions;
        BallisticFunctions fun;

        
        public Queue<double> GetQueueСonvergence()
        {
            return Y;
        }

        public List<List<double>> StartRunge(double tau)
        {
            
            List<double> sol1;
            List<double> sol2;
            if (Tau == 0)
            {
                Y = new Queue<double>();
                sol1 = FindSolutionRungeKutta(tau);
                //WriteToExcel(sol1[5]);
                //y1 = new List<double> { 0,0,0,0,0,0 };
                Y.Enqueue(sol1[5]);
                sol2 = sol1;
                do
                {
                    sol1 = sol2;
                    tau = tau / 2;
                    sol2 = FindSolutionRungeKutta(tau);
                    //WriteToExcel(sol2[5]);
                    Y.Enqueue(sol2[5]);
                } while (!PrecisionCond(sol1, sol2, pRungeKutta));
                RememberTauIfNull(tau);
            }
            else
                sol1 = FindSolutionRungeKutta(Tau);

            return massive;
        }
        private void RememberTauIfNull(double tau)
        {
            if (Tau == 0)
                Tau = tau;
        }
        public static void ResetTau()
        {
            Tau = 0;
        }

        private List<double> FindSolutionRungeKutta(double tau)
        {
            CreateMassive();
            List<double> yn = Fully0();
            while (StopingCondition.Invoke(yn))//условие
            {
                yn = RungeKutta4(yn, tau);
                yn = CorrectingYn(yn);
                FullMassiveN(yn);
            }
            return massive.Last().GetRange(0, massive.Last().Count-1);
        }
        private void CreateMassive()
        {
            massive.Clear();
            massive = new List<List<double>>(Count_y);
            List<double> tmp = new List<double>(init_values.Count);
            for (int i = 0; i < init_values.Count; i++)
                tmp.Add(init_values[i]);
            massive.Add(tmp);
        }
        private List<double> Fully0()
        {
            return massive[0];
        }
        private List<double> RungeKutta4(List<double> yn, double tau)
        {
            FindK(yn, tau);
            return FindYn_p1(yn, tau);
        }
        private void FullMassiveN(List<double> yn)
        {
            List<double> tmp = new List<double>(yn.Count);
            for (int i = 0; i < yn.Count; i++)
                tmp.Add(yn[i]);
            massive.Add(tmp);
        }
        private void FindK(List<double> yn, double tau)
        {
            for (int i = 1; i < 5; i++)
            {
                FindKi(i, DisplaceYnForKi(yn, tau, i));
            }

        }
        private void FindKi(int i, List<double> displace_yn)
        {
            for (int j = 0; j < Count_y; j++)
            {
                K[i, j] = DFunction(displace_yn)[j];
            }
        }
        private double SelectKoef(double tau, int i)
        {
            switch (i)
            {
                case 1: return 0;
                case 2: return tau / 2.0;
                case 3: return tau / 2.0;
                case 4: return tau;
            }
            return 0;
        }
        private List<double> DisplaceYnForKi(List<double> yn, double tau, int i)
        {
            List<double> dipplace_yn = new List<double>(Count_y);
            double koef = SelectKoef(tau, i);
            for (int j = 0; j < Count_y; j++)
            {
                dipplace_yn.Add(yn[j] + koef * K[i - 1, j]);
            }
            return dipplace_yn;
        }
        private List<double> FindYn_p1(List<double> yn, double tau)
        {
            List<double> yn_p1 = new List<double>(Count_y);
            for (int j = 0; j < Count_y; j++)
            {
                yn_p1.Add(yn[j] + tau / 6.0 * (K[1, j] + 2 * K[2, j] + 2 * K[3, j] + K[4, j]));
            }
            return yn_p1;
        }
        private List<double> DFunction(List<double> vars)
        {
            List<double> Df = new List<double>(init_values.Count);
            Df.Add(1.0);
            for (int i = 0; i < init_values.Count - 1; i++)
            {
                Df.Add(functions[i](vars));
            }
            return Df;
        }
        private List<double> CorrectingYn(List<double> yn)
        {
            if (yn[1] > 1) yn[1] = 1;
            if (yn[2] > 1) yn[2] = 1;
            if (yn[3] > 1) yn[3] = 1;
            if (yn[1] < 1) yn[2] = fun.Psi1WhenZ1less1(yn[1]);
            if (yn[4] > yn[6] + ip.Lkm) yn[4] = yn[6] + ip.Lkm;
            return yn;
        }

        private void WriteToExcel(double v)
        {
            using (Excel excel = new Excel(pathexcel))
            {
                excel.OpenFile(0);
                excel.CreateAndChangeNewSheet(v);
                excel.WriteToCell(massive, new List<string> { "t", "z", "psi1", "psi2", "xt", "vsn", "lsn"});
                excel.Save();
                excel.CloseFile();
            }
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using VKR.Helpers;

namespace VKR
{
    static class Graphic
    {
        private static double t(int i, List<List<double>> massive) => GetMassiveValue(i, 0, massive);
        private static double z(int i, List<List<double>> massive) => GetMassiveValue(i, 1, massive);
        private static double psi1(int i, List<List<double>> massive) => GetMassiveValue(i, 2, massive);
        private static double psi2(int i, List<List<double>> massive) => GetMassiveValue(i, 3, massive);
        private static double xt(int i, List<List<double>> massive) => GetMassiveValue(i, 4, massive);
        private static double vsn(int i, List<List<double>> massive) => GetMassiveValue(i, 5, massive);
        private static double lsn(int i, List<List<double>> massive) => GetMassiveValue(i, 6, massive);
        private static double p(int i, List<List<double>> massive) => GetMassiveValue(i, 7, massive);

        private static double GetMassiveValue(int i, int concrectVar, List<List<double>> valsSolRungeKutta)
        {
            return valsSolRungeKutta[i][concrectVar];
        }
        public static Chart DrawGraphicPAndV(this Chart chart,  BallisticFunctions fun, List<List<double>> valsSolRungeKutta)
        {
            chart.Series[0].Points.Clear();
            chart.Series[1].Points.Clear();
            chart.Series[2].Points.Clear();
            chart.Series[3].Points.Clear();
            ConvenientVarsCall f = new ConvenientVarsCall(valsSolRungeKutta);
            for (int i = 0; i < valsSolRungeKutta.Count; i++)
            {
                chart.Series[0].Points.AddXY(f.t(i), PaToMPa(fun.Pkn(f.psi1(i), f.psi2(i), f.xt(i), f.vsn(i), f.lsn(i))));
                chart.Series[1].Points.AddXY(f.t(i), PaToMPa(fun.P(f.psi1(i), f.psi2(i), f.xt(i), f.vsn(i), f.lsn(i))));
                chart.Series[2].Points.AddXY(f.t(i), PaToMPa(fun.IfPLessZero(fun.Psn(f.psi1(i), f.psi2(i), f.xt(i), f.vsn(i), f.lsn(i)))));
                chart.Series[3].Points.AddXY(f.t(i), f.vsn(i));
            }
            chart.ChartAreas[0].AxisX.Maximum = Math.Ceiling(valsSolRungeKutta.Last()[0]);
            return chart;
        }
        private static double PaToMPa(double P)
        {
            return P / 1e6;
        }
        public static Chart DrawGraphicEpureAndV(this Chart chart, BallisticFunctions fun, InputParams ip, List<List<double>> valsSolRungeKutta)
        {
            chart.Series[0].Points.Clear();
            chart.Series[1].Points.Clear();
            ConvenientVarsCall f = new ConvenientVarsCall(valsSolRungeKutta);
            int countX = 20;
            double step = (ip.Lkm + ip.Ldulo) / countX;
            for (int numberX = 0; numberX < countX; numberX++)
            {
                double maxP = 0;
                for (int i = 0; i < valsSolRungeKutta.Count; i++)
                {
                    double pxt = PaToMPa(fun.Pxt(numberX * step, f.psi1(i), f.psi2(i), f.xt(i), f.vsn(i), f.lsn(i)));
                    if (pxt > maxP)
                        maxP = pxt;
                }
                chart.Series[0].Points.AddXY(numberX * step, maxP);
            }
            return chart;
        }
        public static Chart DrawGraphicPsi1AndPsi2(this Chart chart, BallisticFunctions fun, List<List<double>> valsSolRungeKutta)
        {
            chart.Series[0].Points.Clear();
            chart.Series[1].Points.Clear();

            for (int i = 0; i < valsSolRungeKutta.Count; i++)
            {
                chart.Series[0].Points.AddXY(t(i, valsSolRungeKutta), psi1(i, valsSolRungeKutta));
                chart.Series[1].Points.AddXY(t(i, valsSolRungeKutta), psi2(i, valsSolRungeKutta));
            }
            chart.ChartAreas[0].AxisX.Maximum = Math.Ceiling(valsSolRungeKutta.Last()[0]);
            return chart;
        }
        public static Chart DrawErrorRungeKutta(this Chart chart, Queue<double> X, Queue<double> Y, double A, double B, out Sizer sizerout)
        {
            chart.Series[0].Points.Clear();
            chart.Series[1].Points.Clear();
            int CountQueue = Y.Count;
            Sizer sizer = new Sizer();
            double x;
            double y;
            for (int i = 0; i < CountQueue; i++)
            {
                x = X.Dequeue();
                y = Y.Dequeue();
                sizer.MaxMinSize(x, y);
                chart.Series[0].Points.AddXY(x, y);
                chart.Series[1].Points.AddXY(x, A + B * x);
            }
            sizer.SizeChart(chart);
            sizerout = sizer;
            return chart;
        }
        public static Chart DrawСonvergenceRungeKutta(this Chart chart, Queue<double> X, Queue<double> Y,out Sizer sizerout)
        {
            chart.Series[0].Points.Clear();
            chart.Series[1].Points.Clear();
            int CountQueue = Y.Count;
            double x;
            double y;
            Sizer sizer = new Sizer();
            for (int i = 0; i < CountQueue; i++)
            {
                x = X.Dequeue();
                y = Y.Dequeue();
                sizer.MaxMinSize(x, y);
                chart.Series[0].Points.AddXY(x, y);
                chart.Series[1].Points.AddXY(x, y);
            }
            sizer.SizeChart(chart);
            sizerout = sizer;
            return chart;
        }
        public static Series DrawGraphicV(this Series series, BallisticFunctions fun, List<List<double>> valsSolRungeKutta)
        {
            series.Points.Clear();

            for (int i = 0; i < valsSolRungeKutta.Count; i++)
            {
                series.Points.AddXY(t(i, valsSolRungeKutta), vsn(i, valsSolRungeKutta));
            }
            return series;
        }
        public static Series DrawGraphicEpure(this Series series, BallisticFunctions fun, InputParams ip, List<List<double>> valsSolRungeKutta)
        {
            series.Points.Clear();
            ConvenientVarsCall f = new ConvenientVarsCall(valsSolRungeKutta);
            int countX = 20;
            double step = (ip.Lkm + ip.Ldulo) / countX;
            for (int numberX = 0; numberX < countX; numberX++)
            {
                double maxP = 0;
                for (int i = 0; i < valsSolRungeKutta.Count; i++)
                {
                    double pxt = PaToMPa(fun.Pxt(numberX * step, f.psi1(i), f.psi2(i), f.xt(i), f.vsn(i), f.lsn(i)));
                    if (pxt > maxP)
                        maxP = pxt;
                }
                series.Points.AddXY(numberX * step, maxP);
            }
            return series;
        }
        
    }
    class Sizer
    {
        double miny = double.MaxValue;
        double maxy = double.MinValue;
        double minx = double.MaxValue;
        double maxx = double.MinValue;
        public Sizer()
        {

        }
        public void MaxMinSize(double x, double y)
        {
            if (x < minx) minx = x;
            if (x > maxx) maxx = x;
            if (y < miny) miny = y;
            if (y > maxy) maxy = y;
        }
        public Chart SizeChart(Chart chart)
        {
            double intervalx = chart.ChartAreas[0].AxisX.Interval;
            double intervaly = chart.ChartAreas[0].AxisY.Interval;
            chart.ChartAreas[0].AxisX.Minimum = Math.Floor(minx/ intervalx) * intervalx;
            chart.ChartAreas[0].AxisX.Maximum = Math.Ceiling(maxx / intervalx) * intervalx;
            chart.ChartAreas[0].AxisY.Minimum = Math.Floor(miny / intervaly) * intervaly;
            chart.ChartAreas[0].AxisY.Maximum = Math.Ceiling(maxy / intervaly) * intervaly;
            return chart;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKR.Helpers
{
    class MNKParams
    {
        double sumX;
        double sumY;
        double sumXPow2;
        double sumXY;

        double averageX;
        double averageY;
        double averageXPow2;
        double averageXY;


        int CountQueue;

        private Queue<double> _X;
        private Queue<double> _Y;

        public MNKParams(Queue<double>X, Queue<double> Y)
        {
            _X = X;
            _Y = Y;
        }
        private void Sums()
        {
            double x;
            double y;
            for(int i = 0; i < CountQueue; i++)
            {
                x = _X.Dequeue();
                y = _Y.Dequeue();
                sumX += x;
                sumY += y;
                sumXPow2 += Math.Pow(x, 2);
                sumXY += x * y;
            }
        }
        private void Averages()
        {
            averageX = sumX / CountQueue;
            averageY = sumY / CountQueue;
            averageXY = sumXY / CountQueue;
            averageXPow2 = sumXPow2 / CountQueue;
        }
        public void CalculateAverage()
        {
            CountQueue = _Y.Count;
            if (CountQueue.Equals(0)) throw new ArgumentNullException();
            Sums();
            Averages();
        }
        public (double,double) CalculateAB()
        {
            if (CountQueue.Equals(0)) throw new ArgumentNullException();

            double B = (averageXY - averageX * averageY) / (averageXPow2 - Math.Pow(averageX, 2));
            double A = averageY - B * averageX;
            return (A, B);
        }
    }
}

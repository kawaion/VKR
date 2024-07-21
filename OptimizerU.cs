using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VKR.Helpers;

namespace VKR
{
    class OptimizerU
    {
        BallisticFunctions _func;
        RungeKutta _runge;
        double _Pknmax0;
        double _eps;
        double _primaryU;
        double _tau;

        delegate bool Cond(double Pknmaxi);
        public OptimizerU(BallisticFunctions func, RungeKutta runge, double Pknmax0, double eps, double primaryU, double tau)
        {
            _func = func;
            _runge = runge;
            _Pknmax0 = Pknmax0;
            _eps = eps;
            _primaryU = primaryU;
            _tau = tau;
        }
        private double FuncU{
            get 
            {
                    return _func.u11;
            }
            set
            {
                    _func.u11=value;
                _func.u12 = value * 100;
            } 
        }
        
        public double FindOptimizeU()
        {
            FuncU = _primaryU;
            double scaleUp = 1.25, scaleDown = 0.5;//на эти числа будет умножаться u
            //double scaleUp = 1.1, scaleDown = 0.95;
            double uL,uR;
            double PknmaxL, PknmaxR;
            FuncU = FindU11ByCondition(_func, _runge, scaleDown,(Pknmaxi_)=>_Pknmax0>=Pknmaxi_);
            FuncU = FindU11ByCondition(_func, _runge, scaleUp, (Pknmaxi_) => _Pknmax0 <= Pknmaxi_);
            (uL, uR) = FindLeftAndRightU(scaleUp);
            (PknmaxL, PknmaxR) = FindLeftAndRightPknmax(_func,_runge,uL, uR);
            double u = FindUNearPknMax0(_func, _runge, PknmaxL, PknmaxR, uL, uR);
            return u;
        }

        private double FindUNearPknMax0(BallisticFunctions func, RungeKutta runge, double PknmaxL, double PknmaxR, double uL, double uR)
        {
            List<List<double>> valsSolRungeKutta;
            FinderMaxP maxP;
            double Pknmaxi;
            do
            {
                FuncU = (uR - uL) / (PknmaxR - PknmaxL) * (_Pknmax0 - PknmaxL) + uL;
                valsSolRungeKutta = runge.StartRunge(_tau);
                maxP = new FinderMaxP(valsSolRungeKutta, func);
                Pknmaxi = maxP.FindMaxPkn();
                if (Pknmaxi > _Pknmax0)
                {
                    uR = FuncU;
                    PknmaxR = Pknmaxi;
                }
                else
                {
                    uL = FuncU;
                    PknmaxL = Pknmaxi;
                };
            } while (Math.Abs(_Pknmax0 - Pknmaxi) / _Pknmax0 * 100 > _eps);
            return FuncU;
        }

        private (double, double) FindLeftAndRightPknmax(BallisticFunctions func,RungeKutta runge, double uL, double uR)
        {
            List<List<double>> valsSolRungeKutta;
            double PknmaxL, PknmaxR;
            FinderMaxP maxP;
            FuncU = uL;
            valsSolRungeKutta = runge.StartRunge(_tau);
            maxP = new FinderMaxP(valsSolRungeKutta, func);
            PknmaxL = maxP.FindMaxPkn();

            FuncU = uR;
            valsSolRungeKutta = runge.StartRunge(_tau);
            maxP = new FinderMaxP(valsSolRungeKutta, func);
            PknmaxR = maxP.FindMaxPkn();
            return (PknmaxL, PknmaxR);
        }
        private double FindU11ByCondition(BallisticFunctions func, RungeKutta runge, double scale,Cond cond)
        {
            double Pknmaxi = 0;
            List<List<double>> valsSolRungeKutta;
            do
            {
                FuncU *= scale;
                valsSolRungeKutta = runge.StartRunge(_tau);
                FinderMaxP maxP = new FinderMaxP(valsSolRungeKutta, func);
                Pknmaxi = maxP.FindMaxPkn();
            } while (!cond(Pknmaxi));
            return FuncU;
        }
        private (double, double) FindLeftAndRightU(double scaleUp)
        {
            return (FuncU / scaleUp, FuncU);
        }
    }
}

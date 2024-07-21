using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VKR
{
    class CheckPassValues
    {
        public delegate double DelFunctionOptimized(List<double> vars);
        private DelFunctionOptimized _functionOptimized;
        private double _maxSec;

        public CheckPassValues(DelFunctionOptimized functionOptimized,double maxSec)
        {
            _functionOptimized = functionOptimized;
            _maxSec = maxSec;
        }
        public bool IsPassValue(List<double> fParams,out double V)
        {
            Task<double> task = new Task<double>(() => _functionOptimized(fParams));
            task.Start();
            for (int sec = 0; sec < _maxSec; sec++)
            {
                Thread.Sleep(1000);
                if (task.IsCompleted)
                {
                    V = task.Result;
                    return true;
                }
                    
            }
            V = 0;
            return false;
        }
    }
}

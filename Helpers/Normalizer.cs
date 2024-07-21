using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKR.Helpers
{
    static class Normalizer
    {
        /// <summary>
        /// нормализрует список в значения от 0 до 1
        /// </summary>
        /// <param name="x">список значений который надо нормализировать</param>
        /// <param name="a">нижняя граница</param>
        /// <param name="b">верхняя граница</param>
        /// <returns>нормализированный список</returns>
        public static List<double> Normalize(this List<double> x, List<double> a, List<double> b)
        {
            List<double> res = new List<double>(x.Count);
            for (int i = 0; i < a.Count; i++)
                if (b[i] == a[i])
                    res.Add(a[i]);
                else
                    res.Add((x[i] - a[i]) / (b[i] - a[i]));
            return res;
        }
        /// <summary>
        /// анормализирует список в значения от a до b 
        /// </summary>
        /// <param name="x">список значений который надо анормализировать</param>
        /// <param name="a">нижняя граница</param>
        /// <param name="b">верхняя граница</param>
        /// <returns>анормализированный список</returns>
        public static List<double> UNormalize(this List<double> x, List<double> a, List<double> b)
        {
            List<double> res = new List<double>(a.Count);
            for (int i = 0; i < a.Count; i++)
                res.Add(x[i] * (b[i] - a[i]) + a[i]);
            return res;
        }
    }
}

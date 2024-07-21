using System;
using System.Collections.Generic;
using VKR.Helpers;

namespace VKR
{
    class BallisticFunctions
    {
        RungeKutta.DelStopingCondition StopingCondition;
        RungeKutta.DelPrecisionCond PrecisionCondition;

        //double L2;
        public List<double> init_values;
        public InputParams _ip;
        double eps => _ip.epsBF;

        double S01 => Math.PI * D0 * L0 + 7 * (Math.PI * d0 * L0) + 2 * (Math.PI * Math.Pow(D0 / 2, 2) - 7 * Math.PI * Math.Pow(d0 / 2, 2));
        double Lamda01 => Math.PI / 4 * (Math.Pow(D0, 2) - 7 * Math.Pow(d0, 2)) * L0;
        public double omega1;
        public double omega2;
        double omegav => _ip.omegav;
        double d0 => _ip.d0;
        double D0 => 11 * d0;
        double L0 => 2.5 * D0;
        double e1 => d0;
        double delta_powder1 => _ip.delta_powder1;
        double delta_powder2 => _ip.delta_powder2;
        double f1 => _ip.f1;
        double f2 => _ip.f2;
        double alpha1 => _ip.alpha1;
        double alpha2 => _ip.alpha2;
        double teta1 => _ip.teta1;
        double teta2 => _ip.teta2;
        double teta => _ip.teta;
        double Lkm => _ip.Lkm;
        double dkn => _ip.dkn;
        double Skn => Math.PI * Math.Pow(dkn, 2) / 4;
        double Ldulo => _ip.Ldulo;

        public double u11;
        double nu1 => _ip.nu1;
        public double u12;
        double nu2 => _ip.nu2;
        double pf => _ip.pf;

        double Wkm => Skn * Lkm;

        double pn => _ip.pn;
        double kv => _ip.kv;
        double cv => _ip.cv;
        double q => _ip.q;

        double beta => (2 * e1) / L0;
        double P_ => (D0 + 7 * d0) / L0;
        double Q_ => (Math.Pow(D0, 2) - 7 * Math.Pow(d0, 2)) / Math.Pow(L0, 2);
        double kappa => (Q_ + 2 * P_) / Q_ * beta;
        double lamda => 2 * (3 - P_) / (Q_ + 2 * P_) * beta;
        double mu => -6 * Math.Pow(beta, 2) / (Q_ + 2 * P_);
        double psi_s => kappa * (1 + lamda + mu);

        private bool flagPoiasok = true;

        public BallisticFunctions(List<double> x0, InputParams ip)
        {
            _ip = ip;
            u11 = x0[0];
            u12 = x0[1];
            InitMassPowder(u12);
            StopingCondition = StopingCond;
            PrecisionCondition = PrecisionCond;
            double xt = Lkm - omega2 / (Skn * delta_powder2) * (1 - ip.psi20);   
            init_values = new List<double> { ip.t0, ip.z0, ip.psi10, ip.psi20, xt, ip.V0, ip.l0 };
        }

        private void InitMassPowder(double u12)
        {
            if (u12 == 0)
            {
                omega1 = 12;
                omega2 = 12 - omega1;
            }
            else
            {
                omega1 = 7;
                omega2 = 12 - omega1;
            }
        }

        private (double, double, double, double, double, double, double) VParse(List<double> vars)
        {
            return (vars[0], vars[1], vars[2], vars[3], vars[4], vars[5], vars[6]);
        }
        public double DZ1(List<double> vars)
        {
            (double _, double z1, double psi1, double psi2, double xt, double vsn, double lsn) = VParse(vars);
            if (z1 < 1)
                return Uk1(psi1, psi2, xt, vsn, lsn) / e1;
            return 0;
        }
        public double DPsi1(List<double> vars)
        {
            (double _, double _, double psi1, double psi2, double xt, double vsn, double lsn) = VParse(vars);
            double p = P(psi1, psi2, xt, vsn, lsn);
            return S01 * Sigma1(psi1) * Uk1(psi1, psi2, xt, vsn, lsn) / Lamda01;
        }

        public double DPsi2(List<double> vars)
        {
            (double _, double _, double psi1, double psi2, double xt, double vsn, double lsn) = VParse(vars);
            if (omega2 == 0)
                return 0;
            return Skn * delta_powder2 * Uk2(psi1, psi2, xt, vsn, lsn) / omega2;
        }
        public double DXt(List<double> vars)
        {
            (double _, double _, double psi1, double psi2, double xt, double vsn, double lsn) = VParse(vars);
            return vsn + Uk2(psi1, psi2, xt, vsn, lsn);
        }
        public double DVsn(List<double> vars)
        {
            (double t, double _, double psi1, double psi2, double xt, double vsn, double lsn) = VParse(vars);
            if (t == _ip.t0) flagPoiasok = true;
            double psn = Psn(psi1, psi2, xt, vsn, lsn);
            if (psn > pf || !flagPoiasok)
            {
                flagPoiasok = false;
                double vot = Vot(psi1, psi2, xt, vsn, lsn);
                //vot = 0;
                if (omega2 == 0)
                    return (Skn * psn) / (q + (1 - psi2) * omega2);
                return (Skn * psn + vot * omega2 * Skn * delta_powder2 * Uk2(psi1, psi2, xt, vsn, lsn) / omega2) / (q + (1 - psi2) * omega2);
            }
            return 0;
        }
        public double DLsn(List<double> vars)
        {
            (double _, double _, double _, double _, double _, double vsn, double _) = VParse(vars);
            return vsn;
        }
        public double P(double psi1, double psi2, double xt, double vsn, double lsn) 
        {
            double ro_ = Ro_(psi2, xt);
            double thirdPartOfMass = ThirdPartOfMass(psi2);
            double a = thirdPartOfMass * (delta_powder2 - ro_) / ro_ * Math.Pow(u12, 2) / 2.0;
            double b = WVoid(psi1, psi2, lsn)/teta - thirdPartOfMass * vsn * (delta_powder2 - ro_) / ro_ * u12;
            double c = -MassMultiForse(psi1, psi2) + thirdPartOfMass * Math.Pow(vsn, 2) + KineticEnergyShell(psi2, vsn);
            double discriminant = Math.Pow(b, 2) - 4 * a * c;
            double p1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
            double p2 = (-b - Math.Sqrt(discriminant)) / (2 * a);  
            return Math.Max(p1, p2);
        }
        private double WVoid(double psi1, double psi2, double lsn)
        {
            return Wsv(psi1, psi2, lsn) - alpha1 * (omegav + omega1 * psi1) - alpha2 * omega2 * psi2;
        }
        private double MassMultiForse(double psi1, double psi2)
        {
            return (omegav + omega1 * psi1) / teta1 * f1 + omega2 * psi2 / teta2 * f2;
        }
        private double ThirdPartOfMass(double psi2)
        {
            return 1.0 / 3.0 * (omegav + omega1 + psi2 * omega2);
        } 
        private double KineticEnergyShell(double psi2, double vsn)
        {
            return (q + (1 - psi2) * omega2) * Math.Pow(vsn, 2) / 2.0;
        }
        public bool PrecisionCond(List<double> y1, List<double> y2, int p)
        {
            return (Math.Abs((y1[5] - y2[5]) / y2[5]) <= (Math.Pow(2, p + 1) - 2) / 2.0 * eps);
        }
        public bool StopingCond(List<double> vars)
        {
            //return vars[0] <= 0.00573;
            return vars[6] <= Ldulo;
        }
        public double Pxt(double x,double psi1, double psi2, double xt, double vsn, double lsn)
        {
            double pkn = Pkn(psi1, psi2, xt, vsn, lsn);
            double ro_ = Ro_(psi2, xt);
            double pxt = 0;
            if (x-double.Epsilon <= xt)
                pxt = pkn - (omegav + omega1 + omega2 * psi2) / (xt * Skn) * Math.Pow(x / xt, 2) * Math.Pow(vsn - ((delta_powder2 - ro_) / ro_) * Uk2(psi1, psi2, xt, vsn, lsn), 2) / 2.0;
            return pxt;
        }
        public double Vxt(double x, double psi1, double psi2, double xt, double vsn, double lsn)
        {
            double p = P(psi1, psi2, xt, vsn, lsn);
            double ro_ = Ro_(psi2, xt);
            double vxt = 0;
            if (x - double.Epsilon <= xt)
                vxt = (x / xt) * (vsn - ((delta_powder2 - ro_) / ro_) * Uk2(psi1, psi2, xt, vsn, lsn));
            return vxt;
        }
        public double Pkn(double psi1, double psi2, double xt, double vsn, double lsn)
        {
            double p = P(psi1, psi2, xt, vsn, lsn);
            double ro_ = Ro_(psi2, xt);
            return p + ro_ * Math.Pow(vsn - ((delta_powder2 - ro_) / ro_) * Uk2(psi1, psi2, xt, vsn, lsn), 2) / 6.0;
        }
        private double Sigma1(double psi1)
        {
            return (1 + 2 * lamda + 3 * mu) * Math.Sqrt(Math.Abs(1 - psi1) / (1 - psi_s));
        }
        private double Ro_(double psi2, double xt)
        {
            return (omegav + omega1 + omega2 * psi2) / (xt * Skn);
        }
        public double Psn(double psi1, double psi2, double xt, double vsn, double lsn)
        {
            double ro_ = Ro_(psi2, xt);
            double p = P(psi1, psi2, xt, vsn, lsn);
            return p - ro_ * Math.Pow(vsn - ((delta_powder2 - ro_) / ro_) * Uk2(psi1, psi2, xt, vsn, lsn), 2) / 3.0;
        }
        private double Ppr(double vsn)
        {
            return pn * (1 + ((kv + 1) / 4.0) * kv * Math.Pow(vsn / cv, 2) + kv * (vsn / cv) * Math.Sqrt(1 + Math.Pow((kv + 1) / 4.0 * vsn / cv, 2)));
        }
        private double Ro(double psi1, double psi2, double lsn)
        {
            return (omegav + omega1 * psi1 + omega2 * psi2) / Wsv(psi1, psi2, lsn);

        }
        private double Vot(double psi1, double psi2, double xt, double vsn, double lsn)
        {
            double ro = Ro(psi1, psi2, lsn);
            return (delta_powder2 - ro) * Uk2(psi1, psi2, xt, vsn, lsn) / ro;
        }
        private double Uk2(double psi1, double psi2, double xt, double vsn, double lsn)
        {
            double p = P(psi1, psi2, xt, vsn, lsn);
            if (psi2 >= 1)
                return 0;
            return u12 * Math.Pow(p, nu2);

        }
        private double Uk1(double psi1, double psi2, double xt, double vsn, double lsn)
        {
            double p = P(psi1, psi2, xt, vsn, lsn);
            if (psi1 >= 1)
                return 0;
            return u11 * Math.Pow(p, nu1);
        }
        private double Wsv(double psi1, double psi2, double lsn)
        {
            return Wkm + Skn * lsn - omega1 / delta_powder1 * (1 - psi1) - omega2 / delta_powder2 * (1 - psi2);
        }
        public double Psi1WhenZ1less1(double z1)
        {
            return kappa * z1 * (1 + lamda * z1 + mu * Math.Pow(z1, 2));
        }
        public double IfPLessZero(double P)
        {
            if (P < 0)
                return 0;
            else return P;
        }
    }
}

using Newtonsoft.Json;
using System.ComponentModel;

namespace VKR.Helpers
{
    internal class InputParams
    {
        public InputParams()
        {
            
            //epskn = 1E-4;
            //maxDiap = 0;
            //minDiap = 0;
            //u12 = 0;
            //u11 = 0;
            //d0 = 0;
            //tau = 0;
            //Pknmknx0 = 0;
            //Nchromosomes = 0;
            //crossOverChance = 0;
            //mutationChance = 0;
            //counterEps = 0;
            //epsGA = 0;
            //epsBF = 0;
            //omegav = 0;
            //delta_powder1 = 0;
            //delta_powder2=0;
            //f1 = 0;
            //f2 = 0;
            //alpha1 = 0;
            //alpha2 = 0;
            //teta1 = 0;
            //teta2 = 0;
            //teta = 0;
            //dkn = 0;
            //Lkm = 0;
            //Ldulo = 0;
            //v1 = 0;
            //v2 = 0;
            //pf = 0;
            //pn=0;
            //kv = 0;
            //cv = 0;
            //q = 0;
            //t0 = 0;
            //z0 = 0;
            //psi10 = 0;
            //psi20 = 0;
            //V0 = 0;
            //l0 = 0;
            //seed=0;
        }
        [JsonProperty("tau")]
        [Browsable(true), Category("Параметры Рунге-Кутта"), DisplayName("шаг по времени"), Description("шаг по времени")]
        public double tau { get; set; } //= 0.7 * 1e-4;
        [JsonProperty("Pknmknx0")]
        [Browsable(true), Category("Параметры генетического алгоритма"), DisplayName("максимальное давление"), Description("максимальное давление")]
        public double Pknmknx0 { get; set; } //= 500 * 1e6;
        [JsonProperty("Nchromosomes")]
        [Browsable(true), Category("Параметры генетического алгоритма"), DisplayName("популяция"), Description("популяция")]
        public int Nchromosomes { get; set; } //= 100;
        [JsonProperty("crossOverChance")]
        [Browsable(true), Category("Параметры генетического алгоритма"), DisplayName("шанс скрещивания"), Description("шанс скрещивания")]
        public double crossOverChance { get; set; } //= 1;
        [JsonProperty("mutationChance")]
        [Browsable(true), Category("Параметры генетического алгоритма"), DisplayName("шанс мутации"), Description("шанс мутации")]
        public double mutationChance { get; set; } //= 0.01;
        [JsonProperty("counterEps")]
        [Browsable(true), Category("Параметры генетического алгоритма"), DisplayName("минимальное количество поколений"), Description("минимальное количество поколений")]
        public double counterEps { get; set; }//= 10;
        [JsonProperty("epsGA")]
        [Browsable(true), Category("Параметры генетического алгоритма"), DisplayName("погрешность генетического алгоритма"), Description("погрешность генетического алгоритма")]
        public double epsGA { get; set; } //= 0.5;
        [JsonProperty("epsBF")]
        [Browsable(true), Category("Параметры Рунге-Кутта"), DisplayName("погрешность рунге кутты"), Description("погрешность рунге кутты")]
        public double epsBF { get; set; } //= 1e-4;
        [JsonProperty("epskn")]
        [Browsable(true), Category("Параметры генетического алгоритма"), DisplayName("погрешность u11"), Description("погрешность u11")]
        public double epskn { get; set; } //= 1e-4;
        [JsonProperty("maxDiap")]
        [Browsable(true), Category("Параметры генетического алгоритма"), DisplayName("макс диапазон"), Description("задает процент увеличения границы от началных значений варьируемых параметров")]
        public double maxDiap { get; set; } //= 1e-4;
        [JsonProperty("minDiap")]
        [Browsable(true), Category("Параметры генетического алгоритма"), DisplayName("мин диапазон"), Description("задает процент увеличения границы от началных значений варьируемых параметров")]
        public double minDiap { get; set; } //= 1e-4;
        [JsonProperty("omegav")]
        [Browsable(true), Category("Параметры пороха"), DisplayName("масса воспламенителя"), Description("масса воспламенителя")]
        public double omegav { get; set; } //= 0.1;
        [JsonProperty("delta_powder1")]
        [Browsable(true), Category("Параметры пороха"), DisplayName("плотность 1-ой марки пороха"), Description("плотность 1-ой марки пороха")]
        public double delta_powder1 { get; set; } //= 1600;
        [JsonProperty("delta_powder2")]
        [Browsable(true), Category("Параметры пороха"), DisplayName("плотность 2-ой марки пороха"), Description("плотность 2-ой марки пороха")]
        public double delta_powder2 { get; set; } //= 1200;
        [JsonProperty("f1")]
        [Browsable(true), Category("Параметры пороха"), DisplayName("сила 1-ой марки пороха"), Description("сила 1-ой марки пороха")]
        public double f1 { get; set; } //= 11 * 1e5;
        [JsonProperty("f2")]
        [Browsable(true), Category("Параметры пороха"), DisplayName("сила 2-ой марки пороха"), Description("сила 2-ой марки пороха")]
        public double f2 { get; set; }//= 11 * 1e5;
        [JsonProperty("alpha1")]
        [Browsable(true), Category("Параметры пороха"), DisplayName("коволюм 1-ой марки пороха"), Description("коволюм 1-ой марки пороха")]
        public double alpha1 { get; set; } //= 1e-3;
        [JsonProperty("alpha2")]
        [Browsable(true), Category("Параметры пороха"), DisplayName("коволюм 2-ой марки пороха"), Description("коволюм 2-ой марки пороха")]
        public double alpha2 { get; set; } //= 1e-3;
        [JsonProperty("teta1")]
        [Browsable(true), Category("Параметры пороха"), DisplayName("теплоемкость 1-ой марки пороха"), Description("теплоемкость 1-ой марки пороха")]
        public double teta1 { get; set; } //= 0.25;
        [JsonProperty("teta2")]
        [Browsable(true), Category("Параметры пороха"), DisplayName("теплоемкость 2-ой марки пороха"), Description("теплоемкость 2-ой марки пороха")]public double teta2 { get; set; } //= 0.25;
        [JsonProperty("teta")]
        [Browsable(true), Category("Параметры пороха"), DisplayName("теплоемкость смеси порохов"), Description("теплоемкость смеси порохов")]
        public double teta { get; set; } //= 0.25;
        [JsonProperty("dkn")]
        [Browsable(true), Category("Параметры ствола"), DisplayName("диаметр ствола"), Description("диаметр ствола")]
        public double dkn { get; set; } //= 0.125;
        [JsonProperty("Lkm")]
        [Browsable(true), Category("Параметры ствола"), DisplayName("длина каморы"), Description("длина каморы")]
        public double Lkm { get; set; } //= 1;
        [JsonProperty("Ldulo")]
        [Browsable(true), Category("Параметры ствола"), DisplayName("длина дула"), Description("длина дула")]
        public double Ldulo { get; set; } //= 6;
        [JsonProperty("nu1")]
        [Browsable(true), Category("Параметры пороха"), DisplayName("ст давления скорости горения 1-ой марки пороха"), Description("ст давления скорости горения 1-ой марки пороха")]
        public double nu1 { get; set; } //= 1;
        [JsonProperty("nu2")]
        [Browsable(true), Category("Параметры пороха"), DisplayName("ст давления скорости горения 1-ой марки пороха"), Description("ст давления скорости горения 1-ой марки пороха")]
        public double nu2 { get; set; } //= 0;
        [JsonProperty("pf")]
        [Browsable(true), Category("Параметры ствола"), DisplayName("давление форсирования"), Description("давление форсирования")]
        public double pf { get; set; } //= 300 * 1e5;

        [JsonProperty("pn")]
        [Browsable(true), Category("Параметры окр среды"), DisplayName("давление перед ударной волной"), Description("давление перед ударной волной")]
        public double pn { get; set; } //= 1e5;
        [JsonProperty("kv")]
        [Browsable(true), Category("Параметры окр среды"), DisplayName("показатель адиабаты"), Description("показатель адиабаты")]
        public double kv { get; set; } //= 1.4;
        [JsonProperty("cv")]
        [Browsable(true), Category("Параметры окр среды"), DisplayName("скорость звука в воздухе"), Description("скорость звука в воздухе")]
        public double cv { get; set; } //= 340;
        [JsonProperty("q")]
        [Browsable(true), Category("Параметры снаряда"), DisplayName("масса снаряда"), Description("масса снаряда")]
        public double q { get; set; } //= 5;
        [JsonProperty("t0")]
        [Browsable(true), Category("Начальные условия"), DisplayName("время"), Description("время")]
        public double t0 { get; set; }//=0;
        [JsonProperty("z0")]
        [Browsable(true), Category("Начальные условия"), DisplayName("z"), Description("z")]
        public double z0 { get; set; }//=0;
        [JsonProperty("psi10")]
        [Browsable(true), Category("Начальные условия"), DisplayName("доля сгоревшего пороха 1-ой марки пороха"), Description("доля сгоревшего пороха 1-ой марки пороха")]
        public double psi10 { get; set; } //= 0;
        [JsonProperty("psi20")]
        [Browsable(true), Category("Начальные условия"), DisplayName("доля сгоревшего пороха 2-ой марки пороха"), Description("доля сгоревшего пороха 2-ой марки пороха")]
        public double psi20 { get; set; } //= 0;
        [JsonProperty("V0")]
        [Browsable(true), Category("Начальные условия"), DisplayName("скорость снаряда"), Description("скорость снаряда")]
        public double V0 { get; set; } //= 0;
        [JsonProperty("l0")]
        [Browsable(true), Category("Начальные условия"), DisplayName("пройденный путь"), Description("пройденный путь")]
        public double l0 { get; set; } //= 0;
        [JsonProperty("seed")]
        [Browsable(true), Category("Параметры генетического алгоритма"), DisplayName("сид"), Description("сид")]
        public int seed { get; set; }
        [JsonProperty("u12")]
        [Browsable(true), Category("Параметры пороха"), DisplayName("ед скорость горения 2-ой марки пороха"), Description("ед скорость горения 2-ой марки пороха")]
        public double u12 { get; set; }
        [JsonProperty("u11")]
        [Browsable(true), Category("Параметры пороха"), DisplayName("ед скорость горения 1-ой марки пороха"), Description("ед скорость горения 1-ой марки пороха")]
        public double u11 { get; set; }
        [JsonProperty("d0")]
        [Browsable(true), Category("Параметры пороха"), DisplayName("внутренний диаметр каналов"), Description("внутренний диаметр каналов")]
        public double d0 { get; set; }
    }
}

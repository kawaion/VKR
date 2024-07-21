using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Newtonsoft.Json;
using VKR.Helpers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;


namespace VKR
{
    public partial class Form1 : Form
    {
        string pathexcel = Path.GetFullPath(@"graphics.xlsx");
        string pathjson = Path.GetFullPath(@"inputparams.json");
        InputParams ip;
        private List<double> a;
        private List<double> b;
        List<RungeKutta.DelFunctions> functions;
        double U11;
        double maxDiap => ip.maxDiap / 100;
        double minDiap => ip.minDiap / 100;

        private double epskn => ip.epskn;

        private double Pknmknx0 => ip.Pknmknx0;
        double tau => ip.tau;

        public Form1()
        {
            InitializeComponent();
            #region начальные параметры для chart
            chart1.ChartAreas[0].AxisX.Title = "t,мс";
            chart1.ChartAreas[0].AxisY.Title = "P,Мпа";
            chart1.ChartAreas[0].AxisY2.Title = "Vсн,м/мс";

            chart2.ChartAreas[0].AxisX.Title = "x,м";
            chart2.ChartAreas[0].AxisY.Title = "P,МПа";
            //chart2.ChartAreas[0].AxisY2.Title = "v,м/с";

            chart3.ChartAreas[0].AxisX.Title = "t,мс";
            chart3.ChartAreas[0].AxisY.Title = "Ψ";

            chart5.ChartAreas[0].AxisX.Title = "log₁₀τ,мс";
            chart5.ChartAreas[0].AxisY.Title = "log₁₀Vсн,м/мс";

            chart6.ChartAreas[0].AxisX.Title = "log₁₀τ,мс";
            chart6.ChartAreas[0].AxisY.Title = "Vсн,м/мс";

            chart7.ChartAreas[0].AxisX.Title = "t,мс";
            chart7.ChartAreas[0].AxisY.Title = "Vсн,м/мс";

            chart8.ChartAreas[0].AxisX.Title = "x,м";
            chart8.ChartAreas[0].AxisY.Title = "P,МПа";

            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Interval = 1;
            chart1.ChartAreas[0].AxisY2.Interval = 1600/5;
            chart1.ChartAreas[0].AxisY.Minimum = 0;

            chart2.ChartAreas[0].AxisX.Minimum = 0;
            chart2.ChartAreas[0].AxisX.Maximum = 7;
            chart2.ChartAreas[0].AxisX.Interval = 1;
            chart2.ChartAreas[0].AxisY.Minimum = 0;
            //chart2.ChartAreas[0].AxisY2.Interval = 0.002/5;

            chart3.ChartAreas[0].AxisX.Minimum = 0;
            chart3.ChartAreas[0].AxisX.Interval = 1;
            chart3.ChartAreas[0].AxisY.Interval = 0.1;
            chart3.ChartAreas[0].AxisY.Minimum = 0;

            chart5.ChartAreas[0].AxisX.Interval = 1;
            chart5.ChartAreas[0].AxisY.Interval = 1;
            chart5.ChartAreas[0].AxisX.Minimum = -11;

            chart6.ChartAreas[0].AxisX.Interval = 0.0001;
            chart6.ChartAreas[0].AxisY.Interval = 1;

            chart7.ChartAreas[0].AxisX.Minimum = 0;
            chart7.ChartAreas[0].AxisX.Maximum = 6;
            chart7.ChartAreas[0].AxisX.Interval = 1;

            chart8.ChartAreas[0].AxisX.Minimum = 0;
            chart8.ChartAreas[0].AxisX.Maximum = 7;
            chart8.ChartAreas[0].AxisX.Interval = 1;

            textBox1.Text = chart5.ChartAreas[0].AxisY.Interval.ToString();
            textBox2.Text = chart5.ChartAreas[0].AxisX.Interval.ToString();
            textBox3.Text = chart6.ChartAreas[0].AxisY.Interval.ToString();
            textBox4.Text = chart6.ChartAreas[0].AxisX.Interval.ToString();

            #endregion
        }
        private void button1_Click(object sender, EventArgs e)
        {
            List<double> optimRes;
            List<List<double>> solRunge;
            ChooseTypeTask();
            if (WithPowderAttached.Checked)
            {
                GeneticAlgorithm geneticAlgorithm = new GeneticAlgorithm(SolutionGeneticAlgorithm, 2, ip);
                (var res, var leaders) = geneticAlgorithm.Start();
                (optimRes, solRunge) = GetOptimRes(res);
                List<Chromosome> badChromosomes = geneticAlgorithm.GetBadChromosomes();
            }
            else
            {
                Chromosome x = new Chromosome(a);
                (optimRes, solRunge) = GetOptimRes(x);
            }
            WriteToExcel(optimRes, solRunge);
            FullDataGridView();
        }
        private double SolutionGeneticAlgorithm(List<double> x)
        {
            List<double> normX = x.UNormalize(a, b);
            (double V, List<List<double>> _) = SolutionMainTask(normX);
            return V;
        }

        private void WriteToExcel(List<double> optimRes, List<List<double>> solRunge)
        {
            solRunge = ConvertSecToMSec(solRunge);
            using (Excel excel = new Excel(pathexcel))
            {
                excel.OpenFile(0);
                excel.WriteToCell(2, 1, optimRes);
                excel.CreateAndChangeNewSheet(optimRes.Last());
                excel.WriteToCell(solRunge, new List<string> { "t", "z", "psi1", "psi2", "xt", "vsn", "lsn"});
                excel.Save();
                excel.CloseFile();
            }
        }
        private (List<double>, List<List<double>>) GetOptimRes(Chromosome res)
        {
            List<double> optimRes= res.GetParams().UNormalize(a, b);
            (double V,List<List<double>> solRunge) = SolutionMainTask(optimRes);
            optimRes[0] = U11;
            optimRes[1] = U11 * 100;
            optimRes.Add(V);
            return (optimRes,solRunge);
        }
        private void ChooseTypeTask()
        {
            if (WithoutPowderAttached.Checked)a = b = new List<double>() {ip.u11, ip.u12};
            if (WithPowderAttached.Checked)(a, b) = InitializeBorders(); 
        }
        public (double,List<List<double>>) SolutionMainTask(List<double> x0)
        {
            List<List<double>> solRunge;
            BallisticFunctions func = new BallisticFunctions(x0, ip);
            RungeKutta runge = new RungeKutta(
                func.PrecisionCond,
                func.StopingCond,
                func.init_values,
                functions = new List<RungeKutta.DelFunctions> { func.DZ1, func.DPsi1, func.DPsi2, func.DXt, func.DVsn, func.DLsn },
                func
                );
            solRunge = runge.StartRunge(tau);
            if (x0[1] != 0)
            {
                OptimizerU optimizerU = new OptimizerU(func, runge, Pknmknx0, epskn, ip.u11, tau);
                func.u11=optimizerU.FindOptimizeU();

            }
            U11 = func.u11;
            solRunge = runge.StartRunge(tau);
            ConvenientVarsCall f = new ConvenientVarsCall(solRunge);
            double vsn = f.vsn(solRunge.Count - 1);
                FinderMaxP finderMaxP = new FinderMaxP(solRunge, func);
                vsn = f.vsn(finderMaxP.FindIPAtMomentMax(Pknmknx0,ip.epskn));
            return (vsn, solRunge);
        } 
        

        private List<List<double>> ConvertSecToMSec(List<List<double>> valsSolRungeKutta)
        {
            for (int i = 0; i < valsSolRungeKutta.Count; i++)
            {
                valsSolRungeKutta[i][0] *= 1e3;
            }
            return valsSolRungeKutta;
        }
        private void GetInitialDataFromJson()
        {
            var json = File.ReadAllText(pathjson);
            ip = JsonConvert.DeserializeObject<InputParams>(json);
            propertyGrid1.SelectedObject = ip;
        }

        private (List<double>, List<double>) InitializeBorders()
        {
            return (new List<double> { ip.u11 * minDiap, ip.u12 * minDiap},
                    new List<double> { ip.u11 * maxDiap, ip.u12 * maxDiap });
        }
        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            ip = (InputParams)propertyGrid1.SelectedObject;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var res = GetCurrentGridRow();
            (double _,List<List<double>> solRunge) = SolutionMainTask(res.GetParams());
            solRunge = ConvertSecToMSec(solRunge);
            BallisticFunctions func = new BallisticFunctions(res.GetParams(), ip);
            Graphics(func,solRunge);
        }

        private Chromosome GetCurrentGridRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;
            var fparams = new List<double>{(double)dataGridView1.Rows[index].Cells[0].Value,
                                           (double)dataGridView1.Rows[index].Cells[1].Value};
            return new Chromosome(fparams);
        }

        private void Graphics(BallisticFunctions func,List<List<double>> solRunge)
        {
            chart1.DrawGraphicPAndV(func, solRunge);
            chart2.Series[0].DrawGraphicEpure(func,ip, solRunge);
            chart3.DrawGraphicPsi1AndPsi2(func, solRunge);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            GetInitialDataFromJson();
            FullDataGridView();
        }

        private void FullDataGridView()
        {
            List<List<double>> res;
            using (Excel excel = new Excel(pathexcel))
            {
                excel.OpenFile(0);
                res = excel.ReadCellToEnd(2, 1, 3);
                excel.CloseFile();
            }
            int rows = res.Count;
            int cols = res[0].Count;
            dataGridView1.RowCount = rows;
            dataGridView1.ColumnCount = cols;
            for (int n = 0; n < rows; n++)
                for (int i = 0; i < cols; i++)
                    dataGridView1.Rows[n].Cells[i].Value = res[n][i];
        }
        private void button3_Click(object sender, EventArgs e)
        {
            var res = GetCurrentGridRow();
            BallisticFunctions fun = new BallisticFunctions(res.GetParams(),ip);
            RungeKutta.ResetTau();
            var runge = new RungeKutta(
                fun.PrecisionCond,
                fun.StopingCond,
                fun.init_values,
                functions = new List<RungeKutta.DelFunctions> { fun.DZ1, fun.DPsi1, fun.DPsi2, fun.DXt, fun.DVsn, fun.DLsn },
                fun
                );
            _=runge.StartRunge(tau);
            var Y = runge.GetQueueСonvergence();
            var errors = GetErrors(new Queue<double>(Y));
            var X=GetX(tau, new Queue<double>(errors),true);
            MNKParams mnk = new MNKParams(new Queue<double>(X), new Queue<double>(errors));
            mnk.CalculateAverage();
            (var A, var B) = mnk.CalculateAB();
            SetIntervalChart();
            chart5.DrawErrorRungeKutta(new Queue<double>(X), new Queue<double>(errors), A, B, out sizer5);
            chart6.DrawСonvergenceRungeKutta( GetX(tau, new Queue<double>(Y),true), new Queue<double>(Y),out sizer6);
            label1.Text = B.ToString();

        }
        private void SetIntervalChart() {
            chart5.ChartAreas[0].AxisY.Interval = Convert.ToDouble(textBox1.Text);
            chart5.ChartAreas[0].AxisX.Interval = Convert.ToDouble(textBox2.Text);
            chart6.ChartAreas[0].AxisY.Interval = Convert.ToDouble(textBox3.Text);
            chart6.ChartAreas[0].AxisX.Interval = Convert.ToDouble(textBox4.Text);
        }
        private Queue<double> GetErrors(Queue<double> Y)
        {
            int CountY = Y.Count();
            Queue<double> errors = new Queue<double>();
            double endY = 0;
            for (int i = 0; i < CountY; i++)
            {
                endY=Y.Dequeue();
                Y.Enqueue(endY);
            }
            for (int i = 0; i < CountY-1; i++)
            {
                double error = Math.Abs((Y.Dequeue() - endY) / endY);
                errors.Enqueue(Math.Log10(error));
            }
            return errors;
        }

        private Queue<double> GetX(double tau, Queue<double> Y, bool flagLog=false)
        {
            Queue<double> X=new Queue<double>();
            for (int i=0;i<Y.Count;i++)
            {
                double x = tau / Math.Pow(2, i);
                if(flagLog)
                    X.Enqueue(Math.Log10(x)); 
                else
                    X.Enqueue(x);
            }
            return X;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bool flagCorrect = true;
            flagCorrect = CheckExceptionChoose(flagCorrect);
            if (flagCorrect)
            {
                ShowBothResults();
            }
        }

        private bool CheckExceptionChoose(bool flagCorrect)
        {
            while (true)
            {
                if (res1 == null)
                {
                    MessageBox.Show("не выбран первый вариант");
                    flagCorrect = false;
                    break;
                }
                if (res2 == null)
                {
                    MessageBox.Show("не выбран второй вариант");
                    flagCorrect = false;
                    break;
                }
                if (res1.U12 == res2.U12)
                {
                    MessageBox.Show("выбраны одинаковые варианты");
                    flagCorrect = false;
                    break;
                }
                break;
            }

            return flagCorrect;
        }

        private void ShowBothResults()
        {
            FullGridNames();
            BallisticFunctions func;
            FinderMaxP maxP;
            List<List<double>> solRunge;
            double V1, V2;
            (V1,solRunge) = SolutionMainTask(res1.GetParams());
            solRunge = ConvertSecToMSec(solRunge);
            func = new BallisticFunctions(res1.GetParams(), ip);
            maxP = new FinderMaxP(solRunge, func);
            dataGridView2.Rows[1].Cells[1].Value = $"ω1={func.omega1}, ω2={func.omega2}";
            dataGridView2.Rows[1].Cells[2].Value = maxP.FindMaxPkn();
            dataGridView2.Rows[1].Cells[3].Value = maxP.FindMaxP();
            dataGridView2.Rows[1].Cells[4].Value = maxP.FindMaxPsn();
            dataGridView2.Rows[1].Cells[5].Value = V1;
            dataGridView2.Rows[1].Cells[6].Value = "-";
            chart7.Series[0].DrawGraphicV(func, solRunge);
            chart8.Series[0].DrawGraphicEpure(func, ip, solRunge);

            (V2,solRunge) = SolutionMainTask(res2.GetParams());
            solRunge = ConvertSecToMSec(solRunge);
            func = new BallisticFunctions(res2.GetParams(), ip);
            maxP = new FinderMaxP(solRunge, func);
            dataGridView2.Rows[2].Cells[1].Value = $"ω1={func.omega1}, ω2={func.omega2}";
            dataGridView2.Rows[2].Cells[2].Value = maxP.FindMaxPkn();
            dataGridView2.Rows[2].Cells[3].Value = maxP.FindMaxP();
            dataGridView2.Rows[2].Cells[4].Value = maxP.FindMaxPsn();
            dataGridView2.Rows[2].Cells[5].Value = V2;
            dataGridView2.Rows[2].Cells[6].Value = (V1 - V2) / V1 * 100;
            chart7.Series[1].DrawGraphicV(func, solRunge);
            chart8.Series[1].DrawGraphicEpure(func, ip, solRunge);
        }

        private void FullGridNames()
        {
            dataGridView2.RowCount = 3;
            dataGridView2.ColumnCount = 7;
            dataGridView2.Rows[0].Cells[0].Value = "N";
            dataGridView2.Rows[0].Cells[1].Value = "массы порхов";
            dataGridView2.Rows[0].Cells[2].Value = "Pкн max";
            dataGridView2.Rows[0].Cells[3].Value = "P max";
            dataGridView2.Rows[0].Cells[4].Value = "Pсн max";
            dataGridView2.Rows[0].Cells[5].Value = "V д";
            dataGridView2.Rows[0].Cells[6].Value = "dV д";

            dataGridView2.Rows[1].Cells[0].Value = "1";
            dataGridView2.Rows[2].Cells[0].Value = "2";
        }

        Chromosome res1;
        Chromosome res2;
        private Sizer sizer5;
        private Sizer sizer6;

        private void button5_Click(object sender, EventArgs e)
        {
            res1 = GetCurrentGridRow();
            button5.ForeColor = System.Drawing.Color.Green;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            res2 = GetCurrentGridRow();
            button6.ForeColor = System.Drawing.Color.Green;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            res1 = new Chromosome(new List<double> {0,0});
            res2 = new Chromosome(new List<double> {0,0});
            button5.ForeColor = System.Drawing.Color.Black;
            button6.ForeColor = System.Drawing.Color.Black;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SetIntervalChart();
            sizer5.SizeChart(chart5);
            sizer6.SizeChart(chart6);
            
        }
    }
    
}

using System.Collections.Generic;
using OfficeOpenXml;
using System.IO;
using System;

namespace VKR
{
    internal class Excel:IDisposable
    {
        FileInfo file;
        ExcelPackage package;
        ExcelWorkbook wb;
        ExcelWorksheet ws;
        
        
        public Excel(string path)
        {
            file = new FileInfo(path);
        }
        public void OpenFile(int sheet)
        { 
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            if(!file.Exists) throw new FileNotFoundException(); 
            package = new ExcelPackage(file);
            wb = package.Workbook;
            ws = wb.Worksheets[sheet];
        }
        public void ChangeSheet(int Sheet)
        {
            ws = wb.Worksheets[Sheet];
        }
        #region read
        public string ReadCell(int i, int j)
        {
            if (ws.Cells[i, j].Value != null)
                return (string)ws.Cells[i, j].Value;
            else
                return "";
        }
        public double[,] ReadCell(int si, int sj, int ei, int ej)
        {
            Rational r1 = (object[,])ws.Cells[si, sj, ei, ej].Value;
            return (double[,])r1;
        }
        
        public List<List<double>> ReadCellToEnd(int si, int sj, int length)
        {
            int end = si;
            while (ws.Cells[end, sj].Value != null)
                end++;
            Rational r1 = (object[,])ws.Cells[si, sj, end - 1, sj + length - 1].Value;
            return (List<List<double>>)r1;
            //List<List<double>> res=new List<List<double>>();
            
            //for (int i = si; i < end; i++)
            //{
            //    List<double> subres=new List<double>(length);
            //    for(int j=sj;j< sj + length;j++)
            //        subres.Add((double)ws.Cells[i, j].Value); 
            //    res.Add(subres);
            //}
            //return res;
        }
        #endregion

        #region write
        public void WriteToCell(int i, int j, double d)
        {
            ws.Cells[i, j].Value = d;
        }
        public void WriteToCell(List<List<double>> d,List<string> names_params)
        {
            for(int j = 1; j < names_params.Count+1; j++)
            {
                ws.Cells[1, j].Value=names_params[j-1];
            }
            Rational r = d;
            ws.Cells[2, 1, 1+d.Count, d[0].Count].Value = (object[,])r;
        }
        public void WriteToCell(int si, int sj, List<double> d)
        {
            while (ws.Cells[si, sj].Value != null)
                si++;
            Rational r = d;
            ws.Cells[si, sj, si, sj-1+d.Count].Value = (object[,])r;
        }
        #endregion
        public void CreateAndChangeNewSheet(double d)
        {
            wb.Worksheets.Add(d.ToString());
            ws = wb.Worksheets[d.ToString()];
        }
        public void Save()
        {
            package.Save();
        }
        public void SaveAs(string path)
        {
            FileInfo tempfile = new FileInfo(path);
            package.SaveAs(tempfile);
        }
        public void CloseFile()
        {
            package.Dispose();
        }
        public void Dispose()
        {
            package.Dispose();
        }
        public List<List<double>> ConvertToMassiveList(object[,] o)
        {
            List<List<double>> res = new List<List<double>>();
            int rows=o.GetLength(0);
            int cols=o.GetLength(1);
            for (int i = 0; i < rows; i++)
            {
                List<double> subres = new List<double>(cols);
                for (int j = 0; j < cols; j++)
                    subres.Add((double)o[i,j]);
                res.Add(subres);
            }
            return res;
        }
        #region cast
        sealed class Rational
        {
            object[,] rational; 
            private Rational(object[] o)
            {
                object[,] oo = new object[1, o.Length];
                for (int i = 0; i < o.Length; i++)
                    oo[0, i] = o[i];
                rational = oo;
            }
            private Rational(object[,] o)
            {
                rational = o;
            }
            public static implicit operator Rational(object[,] o)
            {
                return new Rational(o);
            }
            public static implicit operator Rational(List<double> d)
            {
                object[] o = new object[d.Count];
                for (int i = 0; i < d.Count; i++)
                    o[i] = d[i];
                return new Rational(o);
            }
            public static implicit operator Rational(List<List<double>> d)
            {
                object[,] o = new object[d.Count,d[0].Count];
                for (int i = 0; i < d.Count; i++)
                    for (int j = 0; j < d[0].Count; j++)
                        o[i,j] = d[i][j];
                return new Rational(o);
            }
            private List<List<double>> ToMassiveList()
            {
                List<List<double>> res = new List<List<double>>();
                int rows = rational.GetLength(0);
                int cols = rational.GetLength(1);
                for (int i = 0; i < rows; i++)
                {
                    List<double> subres = new List<double>(cols);
                    for (int j = 0; j < cols; j++)
                        subres.Add((double)rational[i, j]);
                    res.Add(subres);
                }
                return res;
            }
            private double[,] ToMassiveDouble()
            {
                int rows = rational.GetLength(0);
                int cols = rational.GetLength(1);
                double[,] res = new double[rows,cols];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                        res[i,j]= (double)rational[i, j];
                }
                return res;
            }
            private object[,] ToObject()
            {
                return rational;
            }
            public static explicit operator List<List<double>>(Rational r)
            {
                return r.ToMassiveList();
            }
            public static explicit operator double[,](Rational r)
            {
                return r.ToMassiveDouble();
            }
            public static explicit operator object[,](Rational r)
            {
                return r.ToObject();
            }
        }
        #endregion

    }
}

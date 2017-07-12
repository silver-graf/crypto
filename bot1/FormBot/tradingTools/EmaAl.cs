using FormBot.tools;
using Jojatekok.PoloniexAPI.MarketTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.tradingTools
{
    static class EmaAl
    {
        #region it's fault solve of ema
        //public IList<IEmaDet> EMA(IList<IMarketChartData> ChartMarket)
        //{
        //    bool fl = true;
        //    int i = 0, n = 0;
        //    double sumAver = 0, sumOp = 0, sumCl = 0, k = 0;
        //    n = ChartMarket.Count;
        //    IList<IEmaDet> Listdet = new List<IEmaDet>();

        //    foreach (var cht in ChartMarket)
        //    {
        //        if (fl)
        //        {
        //            sumAver = cht.WeightedAverage;
        //            sumOp = cht.Open;
        //            sumCl = cht.Close;
        //            fl = !fl;
        //            i++;
        //            Listdet.Add(new EmaDet(i, null, cht.Close, cht.Open, cht.Close, cht.Time, cht.VolumeBase, cht.VolumeQuote, cht.High, cht.Low, cht.WeightedAverage));

        //        }
        //        else
        //        {
        //            k = 2.0 / (n + 1);
        //            sumAver = sumAver + (k * (cht.WeightedAverage - sumAver));
        //            sumOp = sumOp + (k * (cht.Open - sumOp));
        //            sumCl = sumCl + (k * (cht.Close - sumCl));
        //            i++;
        //            Listdet.Add(new EmaDet(i, null, sumCl, cht.Open, cht.Close, cht.Time, cht.VolumeBase, cht.VolumeQuote, cht.High, cht.Low, cht.WeightedAverage));
        //        }
        //    }
        //    // IMarketChartData mr = new MarketChartData(DateTime.Now, sumOp, sumCl, 0, 0, 0, 0, sumAver);
        //    // ChartMarket.Add(mr);
        //    return Listdet;
        //}


        //public IList<IEmaDet> EMA2(IList<IMarketChartData> ChartMarket, int step)
        //{          
        //    int i = 0, n = 0;
        //    double sumAver = 0, sumOp = 0, sumCl = 0, k = 0;
        //    n = ChartMarket.Count;
        //    IList<IEmaDet> Listdet = new List<IEmaDet>();
        //    foreach (var cht in ChartMarket)
        //    {
        //        Listdet.Add(new EmaDet(i, null, cht.Close, cht.Open, cht.Close, cht.Time, cht.VolumeBase, cht.VolumeQuote, cht.High, cht.Low, cht.WeightedAverage));
        //    }
        //    k = 2.0 / (step + 1);
        //    for (int j = step; j < ChartMarket.Count; j++)
        //    {
        //        sumAver = 0; sumOp = 0; sumCl = 0; k = 0;
             
        //        sumAver = ChartMarket[j - step].WeightedAverage;
        //        sumOp = ChartMarket[j - step].Open;
        //        sumCl = ChartMarket[j - step].Close;
                
        //        for (int l = 0; l < step; l++)
        //        {                    
        //            sumAver = sumAver + (k * (ChartMarket[j - step + l].WeightedAverage - sumAver));
        //            sumOp = sumOp + (k * (ChartMarket[j - step + l].Open - sumOp));
        //            sumCl = sumCl + (k * (ChartMarket[j - step + l].Close - sumCl));
        //        }
        //        Listdet[j].Ema = sumCl;
        //    }
        //    return Listdet;
        //}
        #endregion


        private static double CalculateFactor(int days)
        {
            if (days < 0)
                return 0;
            return 2.0 / (days + 1);
        }
        private static double Average(double[] data)
        {
            if (data.Length == 0)
                return 0;
            return Sum(data) / data.Length;
        }
        private static double Sum(double[] data)
        {
            double sum = 0;
            foreach (var d in data)
            {
                sum += d;
            }
            return sum;
        }

        public static double[] ExMA(double[] dat, int window)
        {
            if (dat.Length < window)
                return null;

            int diff = dat.Length - window;
            double[] newdata = dat.Take(window).ToArray();
            double factor = CalculateFactor(window);
            double sma = Average(newdata);

            IList<double> result = new List<double>();
            result.Add(sma);

            for (int i = 0; i < diff; i++)
            {
                double prev = result[result.Count - 1];
                double price = dat[window + i];
                double next = factor * (price - prev) + prev;
                result.Add(next);
            }
            return result.ToArray();
        }


        public static IList<IEmaDet> ExMA(IList<IMarketChartData> marketChartData, int window)
        {
            if (marketChartData.Count < window)
                return null;
            
            double[] newdata = marketChartData.Select(i => i.Close).Take(window).ToArray();
            double factor = CalculateFactor(window);
            double sma = Average(newdata);

            IList<IEmaDet> Listdet = new List<IEmaDet>();
            for (int i = 0; i < window; i++)
            {
                Listdet.Add(new EmaDet(i, null, marketChartData[i].Close, marketChartData[i].Open, marketChartData[i].Close, marketChartData[i].Time, marketChartData[i].VolumeBase, marketChartData[i].VolumeQuote, marketChartData[i].High, marketChartData[i].Low, marketChartData[i].WeightedAverage));
            }
            Listdet[window - 1].Ema = sma;
           
            for (int i = window; i < marketChartData.Count; i++)
            {
                double prev = Listdet[Listdet.Count - 1].Ema;
                double price = marketChartData[i].Close;
                double next = factor * (price - prev) + prev;
                Listdet.Add(new EmaDet(i, null, next, marketChartData[i].Open, marketChartData[i].Close, marketChartData[i].Time, marketChartData[i].VolumeBase, marketChartData[i].VolumeQuote, marketChartData[i].High, marketChartData[i].Low, marketChartData[i].WeightedAverage));
            }
            return Listdet;
        }


    }



}

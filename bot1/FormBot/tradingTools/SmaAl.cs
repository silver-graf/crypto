using Jojatekok.PoloniexAPI.MarketTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.tradingTools
{
    static class SmaAl
    {
        public static IList<IMarketChartData> SMA(IList<IMarketChartData> ChartMarket)
        {
            int i = 0;
            double sumAver = 0, sumOp = 0, sumCl = 0;
            foreach (var cht in ChartMarket)
            {
                sumAver += cht.WeightedAverage;
                sumOp += cht.Open;
                sumCl += cht.Close;
                i++;
            }
            sumAver = sumAver / i;
            sumCl = sumCl / i;
            sumOp = sumOp / i;

            IMarketChartData mr = new MarketChartData(DateTime.Now, sumOp, sumCl, 0, 0, 0, 0, sumAver);
            ChartMarket.Add(mr);
            return ChartMarket;
        }

    }
}

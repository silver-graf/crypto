using FormBot.tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jojatekok.PoloniexAPI;

namespace FormBot.tradingTools
{
    class EmaDet : IEmaDet
    {

        public int Num { get; private set; }
        public CurrencyPair Pairs { get; private set; }
        public double Ema { get;  set; }

        public double Open { get; private set; }
        public double Close { get; private set; }

        public DateTime Time { get; private set; }

        public double VolumeBase { get; private set; }
        public double VolumeQuote { get; private set; }

        public double High { get; private set; }
        public double Low { get; private set; }

        public double WeightedAverage { get; private set; }

        public EmaDet(int num, CurrencyPair pairs, double ema, double open, double close, DateTime time, double volumeBase, double volumeQuote, double high, double low, double weightedAverage)
        {
            Num = num;
            Pairs = pairs;
            Ema = ema;
            Open = open;
            Close = close;
            Time = time;
            VolumeBase = volumeBase;
            VolumeQuote = volumeQuote;
            Low = low;
            High = high;
            WeightedAverage = weightedAverage;
        }
    }
}

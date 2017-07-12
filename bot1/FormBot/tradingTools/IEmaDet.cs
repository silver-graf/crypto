using Jojatekok.PoloniexAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.tools
{
    public interface IEmaDet
    {
        int Num { get;  }
        CurrencyPair Pairs { get; }
        double Ema { get; set; }

        double Open { get; }
        double Close { get;  }

        DateTime Time { get; }

        double VolumeBase { get; }
        double VolumeQuote { get;  }

        double High { get; }
        double Low { get;  }

        double WeightedAverage { get; }


    }
}

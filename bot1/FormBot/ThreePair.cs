using Jojatekok.PoloniexAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot
{
    public class ThreePair
    {
        public CurrencyPair FirstPair {get { return firstPair; } private set { } }
        public OrderType Firstpairtype { get { return firstpairtype; } private set { } }
        public CurrencyPair SecondPair { get { return secondPair; } private set { } }
        public OrderType SecondPairtype { get { return secondPairtype; } private set { } }
        public CurrencyPair ThirdPair { get { return thirdPair; } private set { } }
        public OrderType ThirdPairtype { get { return thirdPairtype; } private set { } }
        
         CurrencyPair firstPair;
         OrderType firstpairtype;
         CurrencyPair secondPair;
         OrderType secondPairtype;
         CurrencyPair thirdPair;
         OrderType thirdPairtype;


        public ThreePair(CurrencyPair fp, OrderType fpt, CurrencyPair sp, OrderType spt, CurrencyPair tp, OrderType tpt)
        {
            firstPair = fp;
            secondPair = sp;
            thirdPair = tp;

            firstpairtype = fpt;
            secondPairtype = spt;
            thirdPairtype = tpt;
        }
    }
}

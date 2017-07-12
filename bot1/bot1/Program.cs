using Jojatekok.PoloniexAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bot1
{
    class Program
    {
        private static PoloniexClient PC { get; set; }

        static void Main(string[] args)
        {
            PC = new PoloniexClient("MQSSCZVK-QABNL4LW-D1209Y12-8PB3RNK4", "1f64ddcbe9e9edc0816cfde6bcb40aeb56e08391544a9b36a7f085bcc2b39893ee67b24015de01a34b47bb3961d846049e3723d2fc2e0901a033d1e277ea3590");
            LoadMarketSummaryAsync();
            Console.ReadLine();

        }
        private static async void LoadMarketSummaryAsync()
        {
            var markets = await PC.Markets.GetSummaryAsync();
            var wallets = await PC.Wallet.GetBalancesAsync();
            CurrencyPair c = new CurrencyPair("BTC","ZEC");


            // CurrencyPair c1 = new CurrencyPair("ZEC","BTC" );
            // c.BaseCurrency = "BTC";


            var openOrd = await PC.Markets.GetOpenOrdersAsync(c);
            /*
            foreach (var opord in openOrd.BuyOrders)
            {
                Console.WriteLine(opord.AmountBase.ToString()+" " + opord.AmountQuote +" "+ opord.PricePerCoin);
            }
            */
            var trades = await PC.Trading.GetOpenOrdersAsync(c);
            Console.WriteLine("------------------------------");
            foreach(var tr in trades)
            {
                Console.WriteLine(tr.IdOrder + " " + tr.AmountBase);
            }
            var makeord = await PC.Trading.PostOrderAsync(c, OrderType.Buy, 0.041, 0.004);


            
         //   var tst = PC.Trading.

            





            //foreach (var opo in openOrd)
            //{

            //}

            // var marketstm = await PC.Markets.GetTradesAsync();
            /*
             foreach (var market in markets)
             {
                 Console.WriteLine(market.Key + " price " + market.Value.PriceLast.ToString());
                 Console.WriteLine("--history");
                 //var marketstm = await PC.Markets.GetTradesAsync(market.Key);
                 //foreach (var mar in marketstm)
                 //{
                 //    Console.WriteLine("AmountBase: "+mar.AmountBase+ " AmountQuote:" + mar.AmountQuote+ " PricePerCoin:" + mar.PricePerCoin);

                 //}                
             }
             */
            /*
           Console.WriteLine("------------------------------");
           foreach (var wallet in wallets.Where(i => i.Value.BitcoinValue > 0))
           {
               Console.WriteLine(wallet.Key + "   " + wallet.Value.QuoteAvailable);
           }
           */
            //Console.WriteLine("------------------------------");
            //foreach (var wallet in wallets)
            //{
            //    Console.WriteLine(wallet.Key + "   " + wallet.Value.QuoteAvailable);
            //}




        }
    }
}

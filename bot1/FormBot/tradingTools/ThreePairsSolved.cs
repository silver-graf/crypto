using Jojatekok.PoloniexAPI;
using Jojatekok.PoloniexAPI.MarketTools;
using Jojatekok.PoloniexAPI.WalletTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.tradingTools
{
    class ThreePairsSolved
    {
        //private enum buysell { buy, sell }
        //bool flag = true;
        //bool tmpp = false;
        ////List<ThreePair> Listthird = new List<ThreePair>();
        //IDictionary<CurrencyPair, OrderType> pairlist = new Dictionary<CurrencyPair, OrderType>();
        //ThreePair tp;

        //public List<ThreePair> makeList(IDictionary<CurrencyPair, IMarketData> dataPair)
        //{
        //    List<ThreePair> Listthird = new List<ThreePair>();
        //    foreach (CurrencyPair c in dataPair.Keys)
        //    {
        //        pairlist.Add(c, OrderType.Buy);
        //        pairlist.Add(CurrencyPair.Parse(c.QuoteCurrency + "_" + c.BaseCurrency), OrderType.Sell);
        //    }
        //    foreach (CurrencyPair cp in pairlist.Where(i => i.Value == OrderType.Buy).Select(pr => pr.Key))
        //    {
        //        foreach (CurrencyPair cp1 in pairlist.Where(i => i.Key.BaseCurrency == cp.QuoteCurrency).Select(pr => pr.Key))
        //        {
        //            foreach (CurrencyPair cp2 in pairlist.Where(i => i.Key.BaseCurrency == cp1.QuoteCurrency).Select(pr => pr.Key))
        //            {
        //                if (cp.BaseCurrency == cp2.QuoteCurrency)
        //                {
        //                    tp = new ThreePair(cp, OrderType.Buy, cp1, pairlist[cp1], cp2, pairlist[cp2]);
        //                    Listthird.Add(tp);
        //                }
        //            }
        //        }
        //    }
        //    return Listthird;
        //}

        //public void SolvePair(Task<IDictionary<CurrencyPair, IMarketData>> dataPair)
        //{
        //    wallets = PC.Wallet.GetBalancesAsync();
        //    List<ThreePair> Listthird;
        //    //if (flag)
        //    //{
        //    Listthird = makeList(dataPair.Result);
        //    flag = false;
        //    //}

        //    double fpair = 0, startsum = 1, spair = 0, thpar = 0, fee = 0.025, st = 0;
        //    foreach (ThreePair tp in Listthird)
        //    {

        //        st = wallets.Result[tp.FirstPair.BaseCurrency].BitcoinValue;
        //        fpair = (1 - fee) * startsum / dataPair.Result[tp.FirstPair].OrderTopSell;
        //        spair = (1 - fee) * (tp.SecondPairtype == OrderType.Buy ? fpair / dataPair.Result[tp.SecondPair].OrderTopSell : fpair * dataPair.Result[CurrencyPair.Parse(tp.SecondPair.QuoteCurrency + "_" + tp.SecondPair.BaseCurrency)].OrderTopBuy);
        //        thpar = (1 - fee) * (tp.ThirdPairtype == OrderType.Buy ? spair / dataPair.Result[tp.ThirdPair].OrderTopSell : spair * dataPair.Result[CurrencyPair.Parse(tp.ThirdPair.QuoteCurrency + "_" + tp.ThirdPair.BaseCurrency)].OrderTopBuy);

        //        InfoView.Rows.Add(tp.FirstPair + " => " + tp.SecondPair + " => " + tp.ThirdPair, fpair, spair, thpar);

        //        if (thpar > 1)
        //        {
        //            Loger.SetLog(tp.FirstPair + " => " + tp.SecondPair + " => " + tp.ThirdPair + " " + fpair + " " + spair + " " + thpar + " top orderbuy " + (decimal)dataPair.Result[tp.FirstPair].OrderTopBuy);
        //            if (wallets.Result[tp.FirstPair.BaseCurrency].BitcoinValue > 0 && tmpp == true)
        //            {
        //                makeord = PC.Trading.PostOrderAsync(tp.FirstPair, tp.Firstpairtype, (double)(tp.Firstpairtype == OrderType.Buy ? dataPair.Result[tp.FirstPair].OrderTopBuy : dataPair.Result[tp.FirstPair].OrderTopSell), fpair * (1 + fee)).Result;
        //                Loger.SetLog("makeorder " + makeord.ToString() + " first pair = " + fpair * (1 + fee));
        //                wallets = PC.Wallet.GetBalancesAsync();
        //                Loger.SetLog("wallet " + wallets.Result[tp.FirstPair.QuoteCurrency].BitcoinValue.ToString());

        //                makeord = PC.Trading.PostOrderAsync(tp.SecondPairtype == OrderType.Buy ? tp.SecondPair : CurrencyPair.Parse(tp.SecondPair.QuoteCurrency + "_" + tp.SecondPair.BaseCurrency), tp.SecondPairtype, tp.SecondPairtype == OrderType.Buy ? dataPair.Result[tp.SecondPair].OrderTopBuy : dataPair.Result[CurrencyPair.Parse(tp.SecondPair.QuoteCurrency + "_" + tp.SecondPair.BaseCurrency)].OrderTopSell, spair * (1 + fee)).Result;
        //                Loger.SetLog("makeorder " + makeord.ToString());

        //                wallets = PC.Wallet.GetBalancesAsync();
        //                Loger.SetLog("wallet " + wallets.Result[tp.SecondPairtype == OrderType.Buy ? tp.SecondPair.BaseCurrency : tp.SecondPair.QuoteCurrency].BitcoinValue.ToString());

        //                makeord = PC.Trading.PostOrderAsync(tp.ThirdPairtype == OrderType.Buy ? tp.ThirdPair : CurrencyPair.Parse(tp.ThirdPair.QuoteCurrency + "_" + tp.ThirdPair.BaseCurrency), tp.ThirdPairtype, tp.ThirdPairtype == OrderType.Buy ? dataPair.Result[tp.ThirdPair].OrderTopBuy : dataPair.Result[CurrencyPair.Parse(tp.ThirdPair.QuoteCurrency + "_" + tp.ThirdPair.BaseCurrency)].OrderTopSell, thpar * (1 + fee)).Result;
        //                Loger.SetLog("makeorder " + makeord.ToString());

        //                wallets = PC.Wallet.GetBalancesAsync();
        //                Loger.SetLog("wallet " + wallets.Result[tp.ThirdPairtype == OrderType.Buy ? tp.ThirdPair.BaseCurrency : tp.ThirdPair.QuoteCurrency].BitcoinValue.ToString());
        //                tmpp = false;

        //            }

        //        }
        //    }

        //}

        //public void SolvePairTest(Task<IDictionary<CurrencyPair, IMarketData>> dataPair)
        //{
        //    wallets = PC.Wallet.GetBalancesAsync();
        //    List<ThreePair> Listthird;
        //    //if (flag)
        //    //{
        //    Listthird = makeList(dataPair.Result);
        //    flag = false;
        //    //}

        //    double fpair = 0, startsum = 1, spair = 0, thpar = 0, fee = 0.025, st = 0;


        //    foreach (ThreePair tp in Listthird)
        //    {
        //        st = wallets.Result[tp.FirstPair.BaseCurrency].BitcoinValue;
        //        fpair = (1 - fee) * startsum / dataPair.Result[tp.FirstPair].PriceLast;
        //        spair = (1 - fee) * (tp.SecondPairtype == OrderType.Buy ? fpair / dataPair.Result[tp.SecondPair].PriceLast : fpair * dataPair.Result[CurrencyPair.Parse(tp.SecondPair.QuoteCurrency + "_" + tp.SecondPair.BaseCurrency)].PriceLast);
        //        thpar = (1 - fee) * (tp.ThirdPairtype == OrderType.Buy ? spair / dataPair.Result[tp.ThirdPair].PriceLast : spair * dataPair.Result[CurrencyPair.Parse(tp.ThirdPair.QuoteCurrency + "_" + tp.ThirdPair.BaseCurrency)].PriceLast);

        //        //   InfoView.Rows.Add(tp.FirstPair + " => " + tp.SecondPair + " => " + tp.ThirdPair, fpair, spair, thpar);



        //        //if ( thpar  > 1)
        //        if (thpar > 1)
        //        {
        //            Loger.SetLog(tp.FirstPair + " => " + tp.SecondPair + " => " + tp.ThirdPair + " " + fpair + " " + spair + " " + thpar + " top orderbuy " + (decimal)dataPair.Result[tp.FirstPair].OrderTopBuy + " sell: " + (decimal)dataPair.Result[tp.FirstPair].OrderTopSell + "price last  " + (decimal)dataPair.Result[tp.FirstPair].PriceLast);
        //            //if (wallets.Result[tp.FirstPair.BaseCurrency].BitcoinValue > 0 && tmpp == true)
        //            //{

        //            //    makeord = PC.Trading.PostOrderAsync(tp.FirstPair, tp.Firstpairtype, (double)(tp.Firstpairtype == OrderType.Buy ? dataPair.Result[tp.FirstPair].OrderTopBuy : dataPair.Result[tp.FirstPair].OrderTopSell), fpair * (1 + fee)).Result;
        //            //    Loger.SetLog("makeorder " + makeord.ToString() + " first pair = " + fpair * (1 + fee));



        //            //    wallets = PC.Wallet.GetBalancesAsync();
        //            //    Loger.SetLog("wallet " + wallets.Result[tp.FirstPair.QuoteCurrency].BitcoinValue.ToString());

        //            //    makeord = PC.Trading.PostOrderAsync(tp.SecondPairtype == OrderType.Buy ? tp.SecondPair : CurrencyPair.Parse(tp.SecondPair.QuoteCurrency + "_" + tp.SecondPair.BaseCurrency), tp.SecondPairtype, tp.SecondPairtype == OrderType.Buy ? dataPair.Result[tp.SecondPair].OrderTopBuy : dataPair.Result[CurrencyPair.Parse(tp.SecondPair.QuoteCurrency + "_" + tp.SecondPair.BaseCurrency)].OrderTopSell, spair * (1 + fee)).Result;
        //            //    Loger.SetLog("makeorder " + makeord.ToString());

        //            //    wallets = PC.Wallet.GetBalancesAsync();
        //            //    Loger.SetLog("wallet " + wallets.Result[tp.SecondPairtype == OrderType.Buy ? tp.SecondPair.BaseCurrency : tp.SecondPair.QuoteCurrency].BitcoinValue.ToString());

        //            //    makeord = PC.Trading.PostOrderAsync(tp.ThirdPairtype == OrderType.Buy ? tp.ThirdPair : CurrencyPair.Parse(tp.ThirdPair.QuoteCurrency + "_" + tp.ThirdPair.BaseCurrency), tp.ThirdPairtype, tp.ThirdPairtype == OrderType.Buy ? dataPair.Result[tp.ThirdPair].OrderTopBuy : dataPair.Result[CurrencyPair.Parse(tp.ThirdPair.QuoteCurrency + "_" + tp.ThirdPair.BaseCurrency)].OrderTopSell, thpar * (1 + fee)).Result;
        //            //    Loger.SetLog("makeorder " + makeord.ToString());

        //            //    wallets = PC.Wallet.GetBalancesAsync();
        //            //    Loger.SetLog("wallet " + wallets.Result[tp.ThirdPairtype == OrderType.Buy ? tp.ThirdPair.BaseCurrency : tp.ThirdPair.QuoteCurrency].BitcoinValue.ToString());
        //            //    tmpp = false;

        //            //}

        //        }
        //    }

        //}




   
    }
}

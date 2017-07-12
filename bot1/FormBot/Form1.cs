using Jojatekok.PoloniexAPI;
using Jojatekok.PoloniexAPI.MarketTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Jojatekok.PoloniexAPI.WalletTools;
using FormBot.tools;
using FormBot.tradingTools;
using System.Xml.Linq;
using System.IO;

namespace FormBot
{

    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();

        }
        private static PoloniexClient PC { get; set; }
        private string publApi = "A8CGXC1M-8K3RT6H6-USN5FJNE-PRYMF1MF";
        private string privApi = "5fee105e98df0ef3f4367599a389a8934ec08b0ef34262a5e358e19e1c035eab320d3e9ee392206ec1f785f2c9ad5a307849c2c5b745dcfd92a90532a5b909d5";
        private bool tt = false;
        private Task<IDictionary<CurrencyPair, IMarketData>> markets;
        IList<string> pairsChange;
        IList<string> pairsBase;
        IList<string> pairsQuota;

        ulong makeord;
        private Task<IDictionary<string, Balance>> wallets;

        private void ExchangeButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            InfoView.Rows.Clear();
            InfoView.Columns.Clear();

            markets = PC.Markets.GetSummaryAsync();
            InfoView.Columns.Add("Key", "KEY");
            InfoView.Columns.Add("PriceLast", "PriceLast");
            InfoView.Columns.Add("OrderSpread", "OrderSpread");
            InfoView.Columns.Add("OrderSpreadPercentage", "OrderSpreadPercentage");
            InfoView.Columns.Add("OrderTopBuy", "OrderTopBuy");
            InfoView.Columns.Add("OrderTopSell", "OrderTopSell");
            InfoView.Columns.Add("PriceChangePercentage", "PriceChangePercentage");
            InfoView.Columns.Add("Volume24HourBase", "Volume24HourBase");
            InfoView.Columns.Add("Volume24HourQuote", "Volume24HourQuote");
            foreach (var mar in markets.Result)
            {
                
                InfoView.Rows.Add(mar.Key.ToString(), (decimal)mar.Value.PriceLast, (decimal)mar.Value.OrderSpread, (decimal)mar.Value.OrderSpreadPercentage, (decimal)mar.Value.OrderTopBuy, (decimal)mar.Value.OrderTopSell, (decimal)mar.Value.PriceChangePercentage, (decimal)mar.Value.Volume24HourBase, (decimal)mar.Value.Volume24HourQuote);
            }
            sw.Stop();
            TimeControls.Text = "Time: " + sw.ElapsedMilliseconds / 100.0;
        }

        private void WalletView()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            InfoView.Rows.Clear();
            InfoView.Columns.Clear();

            wallets = PC.Wallet.GetBalancesAsync();
            InfoView.Columns.Add("Key", "KEY");
            InfoView.Columns.Add("BitcoinValue", "BitcoinValue");
            InfoView.Columns.Add("QuoteOnOrders", "QuoteOnOrders");
            foreach (var wal in wallets.Result)
            {
                InfoView.Rows.Add(wal.Key.ToString(), (decimal)wal.Value.BitcoinValue, (decimal)wal.Value.QuoteAvailable, (decimal)wal.Value.QuoteOnOrders);
            }
            sw.Stop();
            TimeControls.Text = "Time: " + sw.ElapsedMilliseconds / 100.0;

            chartWallet.Series["SeriesWallet"].XValueMember = "Key";
            chartWallet.Series["SeriesWallet"].YValueMembers = "BitcoinValue";
            chartWallet.DataSource = wallets.Result.Select((K) => new { Key = K.Key, BitcoinValue = K.Value.BitcoinValue }).Where(b => b.BitcoinValue > 0);


            chartWallet.DataBind();

        }

        private void WalletButton_Click(object sender, EventArgs e)
        {
            WalletView();



        }

        private void Form1_LoadBot(object sender, EventArgs e)
        {
            PC = new PoloniexClient(publApi, privApi);
            PubpicApiKey.Text = publApi;
            PrivateApiKey.Text = privApi;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
           
            markets = PC.Markets.GetSummaryAsync();

            pairsBase = markets.Result.Keys.Select(i => i.BaseCurrency).Distinct().ToList();
            pairsQuota = markets.Result.Keys.Select(i => i.QuoteCurrency).Distinct().OrderBy(s => s).ToList();
            pairsChange = markets.Result.Keys.Select(i => i.QuoteCurrency).Distinct().OrderBy(s => s).ToList();

            BaseCur.DataSource = pairsBase;
            QuoteCur.DataSource = pairsQuota;

            startpair.DataSource = pairsBase;
            startpairex.DataSource = pairsChange;

            sw.Stop();
            TimeControls.Text = "Time: " + sw.ElapsedMilliseconds / 100.0;
            if (tt) makeTime.BackColor = Color.Red; else makeTime.BackColor = Color.Green;
            MPeriod.SelectedIndex = 0;


        }

        private void button1_Click(object sender, EventArgs e)
        {
            PC = new PoloniexClient(PubpicApiKey.Text, PrivateApiKey.Text);
        }


        private void MyOrdr_Click(object sender, EventArgs e)
        {
            InfoView.Rows.Clear();
            InfoView.Columns.Clear();
            InfoView.Columns.Add("Key", "KEY");
            InfoView.Columns.Add("IdOrder", "IdOrder");
            InfoView.Columns.Add("AmountBase", "AmountBase");
            InfoView.Columns.Add("AmountQuote", "AmountQuote");
            InfoView.Columns.Add("PricePerCoin", "PricePerCoin");
            InfoView.Columns.Add("Type", "Type");
            MyOpenOrder(BaseCur.Text + "_" + QuoteCur.Text);

        }

        private void InfoView_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private async void MyOpenOrder(string curencyP)
        {
            if (curencyP != "")
            {
                CurrencyPair c = CurrencyPair.Parse(curencyP);
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                var trades = await PC.Trading.GetOpenOrdersAsync(c);
                if (trades.Count > 0)
                    foreach (var tr in trades)
                    {
                        InfoView.Rows.Add(c.ToString(), tr.IdOrder, (decimal)tr.AmountBase, (decimal)tr.AmountQuote, (decimal)tr.PricePerCoin, tr.Type);
                    }
                sw.Stop();
                TimeControls.Text = "Time: " + sw.ElapsedMilliseconds / 100.0;
            }
            else
            {
                var trades = await PC.Trading.GetOpenOrdersAllAsync();
                //if (trades.Count > 0)
                    //foreach (var tr in trades)
                    //{
                    //    InfoView.Rows.Add( tr.IdOrder, (decimal)tr.AmountBase, (decimal)tr.AmountQuote, (decimal)tr.PricePerCoin, tr.Type);
                    //}

            }
        }

        private void InfoView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //MyOpenOrder(InfoView.Rows[e.RowIndex].Cells[0].Value.ToString());




        }

        private void InfoView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void PrivateApiKey_TextChanged(object sender, EventArgs e)
        {

        }

        private void PubpicApiKey_TextChanged(object sender, EventArgs e)
        {

        }

        private void QuoteCur_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void OpenOrd(string bs, string qt)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            InfoView.Rows.Clear();
            InfoView.Columns.Clear();

            var ord = PC.Markets.GetOpenOrdersAsync(CurrencyPair.Parse(bs + "_" + qt));// GetTradesAsync//  GetOpenOrdersAsync
            InfoView.Columns.Add("AmountBase", "AmountBase");
            InfoView.Columns.Add("PricePerCoin", "PricePerCoin");
            InfoView.Columns.Add("percent", "percent");
            decimal sumper = (decimal)ord.Result.BuyOrders.Sum(i => i.AmountBase);
            decimal perc = 0;

            foreach (var ors in ord.Result.BuyOrders)
            {
                perc = ((decimal)ors.AmountBase / sumper) * 100;

                InfoView.Rows.Add(ors.AmountBase, (decimal)ors.PricePerCoin, perc); //.AmountBase,ors.AmountQuote,(decimal) ors.PricePerCoin);
            }
            sw.Stop();

            TimeControls.Text = "Time: " + sw.ElapsedMilliseconds / 100.0;
        }

        private void openOrd_Click(object sender, EventArgs e)
        {
            OpenOrd(BaseCur.Text, QuoteCur.Text);
        }

        private void makeTime_Click(object sender, EventArgs e)
        {
            tt = !tt;
            timer1.Enabled = tt;
            if (tt) makeTime.BackColor = Color.Red; else makeTime.BackColor = Color.Green;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // timer1_Tick.ev += ExchangeButton_Click;


            //timer1.Tick += EmaAlgo_Click;
            timer1.Tick += button6_Click_1;

        }

        private void exititem_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void startpair_SelectedIndexChanged(object sender, EventArgs e)
        {
            pairsChange = markets.Result.Keys.Where(y => y.BaseCurrency == startpair.SelectedItem.ToString()).Select(i => i.QuoteCurrency).ToList();
            startpairex.DataSource = pairsChange;
            thpairqt.Items.Clear();
            thpairqt.Items.Add(startpair.SelectedItem);
            thpairqt.SelectedIndex = 0;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void startpairex_SelectedIndexChanged(object sender, EventArgs e)
        {
            secpairst.Items.Clear();
            secpairst.Items.Add(startpairex.SelectedItem);
            secpairst.SelectedIndex = 0;
            // pairsChange = markets.Result.Keys.Where(y => y.BaseCurrency == startpairex.SelectedItem.ToString() || y.QuoteCurrency == startpairex.SelectedItem.ToString()).Select(i => i.QuoteCurrency).ToList();
            secpairqt.DataSource = markets.Result.Keys.Where(y => y.BaseCurrency == startpairex.SelectedItem.ToString() || y.QuoteCurrency == startpairex.SelectedItem.ToString()).Select(i => i.QuoteCurrency == startpairex.SelectedItem.ToString() ? i.BaseCurrency : i.QuoteCurrency).ToList();

        }

        private void secpairqt_SelectedIndexChanged(object sender, EventArgs e)
        {
            thpairst.Items.Clear();
            thpairst.Items.Add(secpairqt.SelectedItem);
            thpairst.SelectedIndex = 0;
        }

        private void thpairqt_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrencyPair cp1 = new CurrencyPair(startpair.SelectedItem.ToString(), startpair.SelectedItem.ToString());
            CurrencyPair cp2 = new CurrencyPair(startpair.SelectedItem.ToString(), startpair.SelectedItem.ToString());
            CurrencyPair cp3 = new CurrencyPair(startpair.SelectedItem.ToString(), startpair.SelectedItem.ToString());
        }
        private enum buysell { buy, sell }
        bool flag = true;
        bool tmpp = false;

        IDictionary<CurrencyPair, OrderType> pairlist = new Dictionary<CurrencyPair, OrderType>();
        ThreePair tp;

        public List<ThreePair> makeList(IDictionary<CurrencyPair, IMarketData> dataPair)
        {
            List<ThreePair> Listthird = new List<ThreePair>();
            foreach (CurrencyPair c in dataPair.Keys)
            {
                pairlist.Add(c, OrderType.Buy);
                pairlist.Add(CurrencyPair.Parse(c.QuoteCurrency + "_" + c.BaseCurrency), OrderType.Sell);
            }
            foreach (CurrencyPair cp in pairlist.Where(i => i.Value == OrderType.Buy).Select(pr => pr.Key))
            {
                foreach (CurrencyPair cp1 in pairlist.Where(i => i.Key.BaseCurrency == cp.QuoteCurrency).Select(pr => pr.Key))
                {
                    foreach (CurrencyPair cp2 in pairlist.Where(i => i.Key.BaseCurrency == cp1.QuoteCurrency).Select(pr => pr.Key))
                    {
                        if (cp.BaseCurrency == cp2.QuoteCurrency)
                        {
                            tp = new ThreePair(cp, OrderType.Buy, cp1, pairlist[cp1], cp2, pairlist[cp2]);
                            Listthird.Add(tp);
                        }
                    }
                }
            }
            return Listthird;
        }

      

        public void SolvePairTest(Task<IDictionary<CurrencyPair, IMarketData>> dataPair)
        {
            wallets = PC.Wallet.GetBalancesAsync();
            List<ThreePair> Listthird;
            //if (flag)
            //{
            Listthird = makeList(dataPair.Result);
            flag = false;
            //}

            double fpair = 0, startsum = 1, spair = 0, thpar = 0, fee = 0.025, st = 0;


            foreach (ThreePair tp in Listthird)
            {
                st = wallets.Result[tp.FirstPair.BaseCurrency].BitcoinValue;
                fpair = (1 - fee) * startsum / dataPair.Result[tp.FirstPair].PriceLast;
                spair = (1 - fee) * (tp.SecondPairtype == OrderType.Buy ? fpair / dataPair.Result[tp.SecondPair].PriceLast : fpair * dataPair.Result[CurrencyPair.Parse(tp.SecondPair.QuoteCurrency + "_" + tp.SecondPair.BaseCurrency)].PriceLast);
                thpar = (1 - fee) * (tp.ThirdPairtype == OrderType.Buy ? spair / dataPair.Result[tp.ThirdPair].PriceLast : spair * dataPair.Result[CurrencyPair.Parse(tp.ThirdPair.QuoteCurrency + "_" + tp.ThirdPair.BaseCurrency)].PriceLast);

                //   InfoView.Rows.Add(tp.FirstPair + " => " + tp.SecondPair + " => " + tp.ThirdPair, fpair, spair, thpar);



                //if ( thpar  > 1)
                if (thpar > 1)
                {
                    Loger.SetLog(tp.FirstPair + " => " + tp.SecondPair + " => " + tp.ThirdPair + " " + fpair + " " + spair + " " + thpar + " top orderbuy " + (decimal)dataPair.Result[tp.FirstPair].OrderTopBuy + " sell: " + (decimal)dataPair.Result[tp.FirstPair].OrderTopSell + "price last  " + (decimal)dataPair.Result[tp.FirstPair].PriceLast);
                    //if (wallets.Result[tp.FirstPair.BaseCurrency].BitcoinValue > 0 && tmpp == true)
                    //{

                    //    makeord = PC.Trading.PostOrderAsync(tp.FirstPair, tp.Firstpairtype, (double)(tp.Firstpairtype == OrderType.Buy ? dataPair.Result[tp.FirstPair].OrderTopBuy : dataPair.Result[tp.FirstPair].OrderTopSell), fpair * (1 + fee)).Result;
                    //    Loger.SetLog("makeorder " + makeord.ToString() + " first pair = " + fpair * (1 + fee));



                    //    wallets = PC.Wallet.GetBalancesAsync();
                    //    Loger.SetLog("wallet " + wallets.Result[tp.FirstPair.QuoteCurrency].BitcoinValue.ToString());

                    //    makeord = PC.Trading.PostOrderAsync(tp.SecondPairtype == OrderType.Buy ? tp.SecondPair : CurrencyPair.Parse(tp.SecondPair.QuoteCurrency + "_" + tp.SecondPair.BaseCurrency), tp.SecondPairtype, tp.SecondPairtype == OrderType.Buy ? dataPair.Result[tp.SecondPair].OrderTopBuy : dataPair.Result[CurrencyPair.Parse(tp.SecondPair.QuoteCurrency + "_" + tp.SecondPair.BaseCurrency)].OrderTopSell, spair * (1 + fee)).Result;
                    //    Loger.SetLog("makeorder " + makeord.ToString());

                    //    wallets = PC.Wallet.GetBalancesAsync();
                    //    Loger.SetLog("wallet " + wallets.Result[tp.SecondPairtype == OrderType.Buy ? tp.SecondPair.BaseCurrency : tp.SecondPair.QuoteCurrency].BitcoinValue.ToString());

                    //    makeord = PC.Trading.PostOrderAsync(tp.ThirdPairtype == OrderType.Buy ? tp.ThirdPair : CurrencyPair.Parse(tp.ThirdPair.QuoteCurrency + "_" + tp.ThirdPair.BaseCurrency), tp.ThirdPairtype, tp.ThirdPairtype == OrderType.Buy ? dataPair.Result[tp.ThirdPair].OrderTopBuy : dataPair.Result[CurrencyPair.Parse(tp.ThirdPair.QuoteCurrency + "_" + tp.ThirdPair.BaseCurrency)].OrderTopSell, thpar * (1 + fee)).Result;
                    //    Loger.SetLog("makeorder " + makeord.ToString());

                    //    wallets = PC.Wallet.GetBalancesAsync();
                    //    Loger.SetLog("wallet " + wallets.Result[tp.ThirdPairtype == OrderType.Buy ? tp.ThirdPair.BaseCurrency : tp.ThirdPair.QuoteCurrency].BitcoinValue.ToString());
                    //    tmpp = false;

                    //}

                }
            }

        }


        private void button3_Click(object sender, EventArgs e)
        {
            double fpair = 0, startsum = 0, spair = 0, thpar = 0;
            CurrencyPair cp1 = new CurrencyPair(startpair.SelectedItem.ToString(), startpair.SelectedItem.ToString());
            CurrencyPair cp2 = new CurrencyPair(startpair.SelectedItem.ToString(), startpair.SelectedItem.ToString());
            CurrencyPair cp3 = new CurrencyPair(startpair.SelectedItem.ToString(), startpair.SelectedItem.ToString());



            startsum = double.Parse(textBox1.Text.ToString());

            fpair = (markets.Result.ContainsKey(CurrencyPair.Parse(startpairex.SelectedItem.ToString() + '_' + startpair.SelectedItem.ToString())) ? markets.Result[CurrencyPair.Parse(startpairex.SelectedItem.ToString() + '_' + startpair.SelectedItem.ToString())].PriceLast : markets.Result[CurrencyPair.Parse(startpair.SelectedItem.ToString() + '_' + startpairex.SelectedItem.ToString())].PriceLast);
            label2.Text = ((decimal)fpair).ToString();
            spair = (markets.Result.ContainsKey(CurrencyPair.Parse(secpairst.SelectedItem.ToString() + '_' + secpairqt.SelectedItem.ToString())) ? markets.Result[CurrencyPair.Parse(secpairst.SelectedItem.ToString() + '_' + secpairqt.SelectedItem.ToString())].PriceLast : markets.Result[CurrencyPair.Parse(secpairqt.SelectedItem.ToString() + '_' + secpairst.SelectedItem.ToString())].PriceLast);
            label3.Text = ((decimal)spair).ToString();
            thpar = (markets.Result.ContainsKey(CurrencyPair.Parse(thpairst.SelectedItem.ToString() + '_' + thpairqt.SelectedItem.ToString())) ? markets.Result[CurrencyPair.Parse(thpairst.SelectedItem.ToString() + '_' + thpairqt.SelectedItem.ToString())].PriceLast : markets.Result[CurrencyPair.Parse(thpairqt.SelectedItem.ToString() + '_' + thpairst.SelectedItem.ToString())].PriceLast);
            label4.Text = ((decimal)thpar).ToString();

            label7.Text = ((1 - 0.025) * startsum / fpair).ToString();
            label6.Text = ((1 - 0.025) * (startsum / fpair) * spair).ToString();
            label5.Text = ((1 - 0.025) * ((startsum / fpair) * spair) * thpar).ToString();

            //PostMailer.SendMail("smtp.gmail.com", "silverrrgraf@gmail.com", "Aa03071973", "selivanovsky@mail.ua", "label5.Text = "+ label5.Text, "Тело письма");



        }

        private void button4_Click(object sender, EventArgs e)
        {
            //InfoView.Rows.Clear();
            //InfoView.Columns.Clear();
            //InfoView.Columns.Add("pairs", "pairs");
            //InfoView.Columns.Add("firstPair", "firstPair");
            //InfoView.Columns.Add("secondPair", "secondPair");
            //InfoView.Columns.Add("thirdPair", "thirdPair");
            SolvePairTest(markets);
        }

        private void log_time_Tick(object sender, EventArgs e)
        {
            //InfoView.Rows.Clear();
            //InfoView.Columns.Clear();
            //InfoView.Columns.Add("pairs", "pairs");
            //InfoView.Columns.Add("firstPair", "firstPair");
            //InfoView.Columns.Add("secondPair", "secondPair");
            //InfoView.Columns.Add("thirdPair", "thirdPair");

            markets = PC.Markets.GetSummaryAsync();
            SolvePairTest(markets);


        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        public void infoPeriod()
        {
            InfoView.Rows.Clear();
            InfoView.Columns.Clear();
            InfoView.Columns.Add("num", "num");
            InfoView.Columns.Add("pairs", "pairs");
            InfoView.Columns.Add("High", "High");
            InfoView.Columns.Add("Low", "Low");
            InfoView.Columns.Add("Time", "Time");
            InfoView.Columns.Add("VolumeBase", "VolumeBase");
            InfoView.Columns.Add("VolumeQuote", "VolumeQuote");
            InfoView.Columns.Add("WeightedAverage", "WeightedAverage");
            InfoView.Columns.Add("Open", "Open");
            InfoView.Columns.Add("Close", "Close");
            InfoView.Columns.Add("Period1", "Period1");
            InfoView.Columns.Add("Period2", "Period2");
            InfoView.Columns.Add("Profit", "Profit");
            InfoView.Columns.Add("Dif", "Dif");
            InfoView.Columns.Add("Profit2", "Profit2");
            InfoView.Columns.Add("ProfitPer", "ProfitPer");
        }

        private void SmaAlgo_Click(object sender, EventArgs e)
        {
            infoPeriod();
            int d = int.Parse(numDay.Text);
            int mPer = GraphsPer(MPeriod.SelectedIndex);
            CurrencyPair cp = new CurrencyPair("USDT", "BTC");

            var er = PC.Markets.GetChartDataAsync(cp, (MarketPeriod)mPer, DateTime.Now.AddDays(-d), DateTime.Now);
            int i = 0;
            foreach (var ter in SmaAl.SMA(er.Result))
            {
                i++;
                InfoView.Rows.Add(i, cp.ToString(), (decimal)ter.Open, (decimal)ter.Close, (decimal)ter.High, (decimal)ter.Low, ter.Time, (decimal)ter.VolumeBase, (decimal)ter.VolumeQuote, (decimal)ter.WeightedAverage);
            }
            InfoView.Rows[InfoView.RowCount - 2].DefaultCellStyle.BackColor = Color.AliceBlue;
        }
        private int GraphsPer(int g)
        {
            int mPer;
            switch (g)
            {
                case 0:
                    mPer = 300;
                    break;
                case 1:
                    mPer = 900;
                    break;
                case 2:
                    mPer = 1800;
                    break;
                case 3:
                    mPer = 7200;
                    break;
                case 4:
                    mPer = 14400;
                    break;
                case 5:
                    mPer = 86400;
                    break;
                default:
                    mPer = 86400;
                    break;
            }
            return mPer;
        }


        static double SolveExchange(OrderType ot, double amount, double price, double fee)
        {
            //double fee = 0.025;
            Loger.SetLog(ot + " amount: " + amount + " price: " + price);
            return (1 - fee) * (ot == OrderType.Buy ? amount / price : amount * price);            
        }

        

        static double StratEma1021(double emaShort ,double emalong)
        {
            return 100 * (emaShort - emalong) / ((emaShort + emalong) / 2);
        }

     
        
        private void EmaAlgo_Click(object sender, EventArgs e)
        {
            double testProfit = 1, profit2 = 1, profitper = 2;
            
            bool fl = true , fl2 = true;
            infoPeriod();
            int mPer = GraphsPer(MPeriod.SelectedIndex);
            int d = int.Parse(numDay.Text);
            int per1 = int.Parse(Periods.Text);
            int per2 = int.Parse(Periods2.Text);
            double diff, diffOld;
            
            CurrencyPair cp = new CurrencyPair(BaseCur.Text, QuoteCur.Text);
            Task<IList<IMarketChartData>> er;
            if (DayPeriodCheck.Checked)
            {
                 er= PC.Markets.GetChartDataAsync(cp, (MarketPeriod)mPer, DateTime.Now.AddDays(-d), DateTime.Now);
            }
            else
            {
                 er = PC.Markets.GetChartDataAsync(cp, (MarketPeriod)mPer, dateTimePicker1.Value, dateTimePicker2.Value);
            }
            var emas1 = EmaAl.ExMA(er.Result, per1);
            var emas2 = EmaAl.ExMA(er.Result, per2);
            for (int i = 0; i < er.Result.Count; i++)
            {
                InfoView.Rows.Add(i, cp.ToString(), (decimal)emas1[i].High, (decimal)emas1[i].Low, emas1[i].Time, (decimal)emas1[i].VolumeBase, (decimal)emas1[i].VolumeQuote, (decimal)emas1[i].WeightedAverage, (decimal)emas1[i].Open, (decimal)emas1[i].Close, (decimal)emas1[i].Ema, (decimal)emas2[i].Ema);
                if (emas1[i].Ema > emas2[i].Ema)
                {
                    InfoView.Rows[i].DefaultCellStyle.BackColor = Color.Teal;
                    if (fl && i > 0 ? emas1[i - 1].Ema <= emas2[i - 1].Ema : false)
                    {
                        testProfit = SolveExchange(OrderType.Sell, testProfit, emas1[i].Open,0.025);
                        fl = !fl;
                    }
                }
                else
                {
                    InfoView.Rows[i].DefaultCellStyle.BackColor = Color.Silver;
                    if (fl != true && i > 0 ? emas1[i - 1].Ema > emas2[i - 1].Ema : false)
                    {
                        testProfit = SolveExchange(OrderType.Buy, testProfit, emas1[i].Open,0.025);
                        fl = !fl;
                    }
                }
                diff = StratEma1021(emas1[i].Ema, emas2[i].Ema);
                diffOld = i>0? StratEma1021(emas1[i - 1].Ema, emas2[i - 1].Ema):0;
                InfoView.Rows[i].Cells[12].Value = testProfit;
                InfoView.Rows[i].Cells[13].Value = diff;

                if (diff >0.25 && diffOld<0 && fl2 && i > Math.Max(per1, per2))
                {
                    profit2 = SolveExchange(OrderType.Sell, profit2, emas1[i].Close,0.025);
                    fl2 = !fl2;
                }
                if ((diff < 0 || (profit2/100)* profitper < emas1[i].Close) && diffOld > 0 && fl2 == false  && i >Math.Max(per1,per2) )
                {
                    profit2 = SolveExchange(OrderType.Buy, profit2, emas1[i].Close,0.025);
                    fl2 = !fl2;
                }
                InfoView.Rows[i].Cells[14].Value = profit2;
                InfoView.Rows[i].Cells[15].Value = (profit2 / 100) * profitper;
            }





        }



        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {

        }

        void spyer(IList<IMarketChartData> marketChartData )
        {
            bool fl= true;
            for (int i = 0;i<marketChartData.Count;i++)
            {
                if (fl)
                {
                    fl = !fl;
                }
                else
                {
                    //marketChartData[i].High

                }

            }
        }
      
  

        private void SpyButton_Click(object sender, EventArgs e)
        {
            double testProfit = 1, profit2 = 1, profitper = 2;
            bool fl = true, fl2 = true;
            infoPeriod();
            int mPer = GraphsPer(MPeriod.SelectedIndex);
            int d = int.Parse(numDay.Text);
            int per1 = int.Parse(Periods.Text);
            int per2 = int.Parse(Periods2.Text);
           
            Task<IList<IMarketChartData>> er;
            double diff, diffOld;
            CurrencyPair cp;
            foreach (string bs in pairsBase)
            {
                foreach (string qt in pairsQuota )
                {
                    cp= new CurrencyPair(bs, qt);
                   
                    if (DayPeriodCheck.Checked)
                    {
                        er = PC.Markets.GetChartDataAsync(cp, (MarketPeriod)mPer, DateTime.Now.AddDays(-d), DateTime.Now);
                    }
                    else
                    {
                        er = PC.Markets.GetChartDataAsync(cp, (MarketPeriod)mPer, dateTimePicker1.Value, dateTimePicker2.Value);
                    }
                    
                }
            }
            


        }


        //        BarList bl = new BarList(BarInterval.Day);
        //        bl.DayFromGoogle("IBM");
        //int lookback = 14;
        //        decimal sumadvance = 0;
        //        decimal sumdecline = 0;

        //for (int i = bl.NumBars() - 1; (bl.NumBars()>=lookback) && (i>bl.NumBars()-1-lookback); i--)

        //{
        //Bar current = bl.Get(i);
        //        Bar prevday = bl.Get(i - 1);

        //if (current.Close<prevday.Close)
        //sumdecline += prevday.Close - current.Close;
        //else if (current.Close > prevday.Close)
        //sumadvance += current.Close - prevday.Close;

        //}

        //    decimal avggain = sumadvance / lookback;
        //    decimal avgloss = sumdecline / lookback;
        //    decimal rs = avggain / avgloss;
        //    decimal rsi = 100 - (100 / (1 + rs));


        public static IList<IEmaDet> RSI(IList<IMarketChartData> marketChartData, int window)
        {
            if (marketChartData.Count < window)
                return null;

            double[] newdata = marketChartData.Select(i => i.Close).Take(window).ToArray();
           
            IList<IEmaDet> Listdet = new List<IEmaDet>();
                       
            double sumadvance = 0;
            double sumdecline = 0;
            double current = 0;
            double prevday = 0;
            double avgloss;
            double rs;
            double rsi;
            double avggain;

            for (int i = 0; i < window; i++)
            {
                Listdet.Add(new EmaDet(i, null, 0, marketChartData[i].Open, marketChartData[i].Close, marketChartData[i].Time, marketChartData[i].VolumeBase, marketChartData[i].VolumeQuote, marketChartData[i].High, marketChartData[i].Low, marketChartData[i].WeightedAverage));
            }

            for (int i = window; i < marketChartData.Count; i++)
            {
                for (int j = i- window+1;j<i;j++)
                {
                    current = marketChartData[j].Close;
                    prevday = marketChartData[j - 1].Close;
                    if (current < prevday)
                        sumdecline += prevday - current;
                    else if (current > prevday)
                        sumadvance += current - prevday;
                }
                 avggain = sumadvance / window;
                 avgloss = sumdecline / window;
                if (avgloss != 0)
                {
                    rs = avggain / avgloss;
                }
                else
                {
                    rs = 100;
                }
                 rsi = 100 - (100 / (1 + rs));
                Listdet.Add(new EmaDet(i, null, rsi, marketChartData[i].Open, marketChartData[i].Close, marketChartData[i].Time, marketChartData[i].VolumeBase, marketChartData[i].VolumeQuote, marketChartData[i].High, marketChartData[i].Low, marketChartData[i].WeightedAverage));


            }


            return Listdet;

        }


        private void button5_Click_1(object sender, EventArgs e)
        {
            double testProfit = 1, profit2 = 1, profitper = 2;
            bool fl = true, fl2 = true;
            infoPeriod();
            int mPer = GraphsPer(graphsper2.SelectedIndex);
            int d = int.Parse(perdayrs.Text);
            int inrv = int.Parse(intervRSI.Text);
          
            double diff, diffOld;

            CurrencyPair cp = new CurrencyPair(BaseCur.Text, QuoteCur.Text);
            IList<IMarketChartData> er;
            er = PC.Markets.GetChartDataAsync(cp, (MarketPeriod)mPer, DateTime.Now.AddDays(-d), DateTime.Now).Result;
           
            var r = RSI(er, inrv);
            for (int i =0;i< r.Count;i++ )
            {
                InfoView.Rows.Add(i, cp.ToString(), (decimal)r[i].High, (decimal)r[i].Low, r[i].Time, (decimal)r[i].VolumeBase, (decimal)r[i].VolumeQuote, (decimal)r[i].WeightedAverage, (decimal)r[i].Open, (decimal)r[i].Close, (decimal)r[i].Ema);

            }

            for (int j = 2; j <= 25; j++)
            {
                Loger.SetLog("----------- "+j+" ---------------- ");
                var l = RSI(er, j);
                for (int i = 0; i < l.Count; i++)
                {
                    Loger.SetLog(l[i].Time+" "+ string.Format("{0:0.000}", ((decimal)l[i].Close)) + " "+l[i].Ema);

                }
                Loger.SetLog("--------------------------- \r\n");
            }






        }
        static bool checkLastPeriod( IList<IMarketChartData> data)
        {            
            double start = data.Last().Open;
            double ends = data.Last().Close;
            return start < ends ? true : false; 
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            double solveex = 0, profit =0.1 , newsolveex = 0 ,newprice = 0 , startamount =1, fee = 0.0025;

            infoPeriod();
            int mPer = GraphsPer(graphsper2.SelectedIndex);
            int d = int.Parse(perdayrs.Text);
            int inrv = int.Parse(intervRSI.Text);  
                      

            CurrencyPair cp = new CurrencyPair(BaseCur.Text, QuoteCur.Text);
            IList<IList<IMarketChartData>> er = new List<IList<IMarketChartData>>();
            for (int i = 0; i < 3; i++)
            {
                er.Add(PC.Markets.GetChartDataAsync(cp, (MarketPeriod)GraphsPer(i), DateTime.Now.AddHours(-5), DateTime.Now).Result);
            }
            if( checkLastPeriod(er[0]) && checkLastPeriod(er[1]) && checkLastPeriod(er[2]))
            {
                solveex = SolveExchange(OrderType.Sell, startamount, er[0].Last().Close,fee);
                newsolveex = ((solveex / 100) * profit) + solveex;
                newprice = newsolveex / startamount;
                Loger.SetLog(cp + " "+ (decimal)er[0].Last().Close + " solveex: "+ solveex+ " time "+ er[0].Last().Time + " want: " + (decimal)newsolveex +" newprice: "+ (decimal)newprice);

            }
        }


        void daySolveStrat( )
        {
            decimal mx, mn, aver, averpol, ma;
            markets = PC.Markets.GetSummaryAsync();
            IList<IMarketChartData> er;
            foreach (var mar in markets.Result.Where(i => i.Key.BaseCurrency == "BTC" && i.Value.Volume24HourBase > 200))
            {

                

                er = PC.Markets.GetChartDataAsync(mar.Key, (MarketPeriod)GraphsPer(0), DateTime.Now.AddHours(-24), DateTime.Now).Result;

                mx = (decimal)er.Max(i => i.High);
                mn = (decimal)er.Min(i => i.Low);
                aver = (mx / (mn / 100)) - 100;
                ma = (mx + mn) / 2;
                
            }



        }





        private void dayStrat_Click(object sender, EventArgs e)
        {
            
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            InfoView.Rows.Clear();
            InfoView.Columns.Clear();
            decimal mx, mn,aver,averpol,ma, endpr, endprper;
            markets = PC.Markets.GetSummaryAsync();
            IList<IMarketChartData> er;
            var ord = PC.Markets.GetOpenOrdersAsync(CurrencyPair.Parse(BaseCur.Text+"_"+QuoteCur.Text), 100);
            InfoView.Columns.Add("Key", "KEY");
            InfoView.Columns.Add("PriceLast", "PriceLast");
            InfoView.Columns.Add("OrderSpread", "OrderSpread");
            InfoView.Columns.Add("OrderSpreadPercentage", "OrderSpreadPercentage");
            InfoView.Columns.Add("OrderTopBuy", "OrderTopBuy");
            InfoView.Columns.Add("OrderTopSell", "OrderTopSell");
            InfoView.Columns.Add("PriceChangePercentage", "PriceChangePercentage");
            InfoView.Columns.Add("Volume24HourBase", "Volume24HourBase");
            InfoView.Columns.Add("Volume24HourQuote", "Volume24HourQuote");
            InfoView.Columns.Add("High", "High");
            InfoView.Columns.Add("Low", "Low");
            InfoView.Columns.Add("aver", "aver");
            InfoView.Columns.Add("ma", "ma");
            InfoView.Columns.Add("endpr", "endpr");
            InfoView.Columns.Add("endprper", "endprper");

            //var mar = markets.Result.Where(i => i.Key.BaseCurrency == "BTC" && i.Value.Volume24HourBase > 200 && i.Value.PriceChangePercentage > 0.1).OrderByDescending(p => p.Value.PriceChangePercentage).First();
            

            foreach (var mar in markets.Result.Where(i => i.Key.BaseCurrency == "BTC" && i.Value.Volume24HourBase > 200 && i.Value.PriceChangePercentage>0.1).OrderByDescending(p=>p.Value.PriceChangePercentage))
            {

            er = PC.Markets.GetChartDataAsync(mar.Key, (MarketPeriod)GraphsPer(4), DateTime.Now.AddHours(-24), DateTime.Now).Result;

                mx = (decimal)er.Last().High;//(i => i.High);
                mn = (decimal)er.Last().Low; //Min(i => i.Low);
                aver = (mx / (mn / 100)) - 100;
                ma = (mx + mn) / 2;

                endpr = ((mx + (decimal)mar.Value.PriceLast) / 2) * 1.005M;
                endprper = (decimal)mar.Value.PriceLast * 1.01M;
                InfoView.Rows.Add(mar.Key.ToString(), (decimal)mar.Value.PriceLast, (decimal)mar.Value.OrderSpread, (decimal)mar.Value.OrderSpreadPercentage, (decimal)mar.Value.OrderTopBuy, (decimal)mar.Value.OrderTopSell, (decimal)mar.Value.PriceChangePercentage, (decimal)mar.Value.Volume24HourBase, (decimal)mar.Value.Volume24HourQuote, mx, mn, aver, ma, endpr, endprper);
            }



            sw.Stop();
            TimeControls.Text = "Time: " + sw.ElapsedMilliseconds / 100.0;


        }

        private void Ecxhange_Selected(object sender, TabControlEventArgs e)
        {

        }

        private void Ecxhange_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Ecxhange.SelectedIndex == 5)
            {
                ExchangeButton_Click(sender,e);
            }           
            if (Ecxhange.SelectedIndex == 6)
            {
                WalletView();
            }
            if (Ecxhange.SelectedIndex == 7)
            {
                InfoView.Rows.Clear();
                InfoView.Columns.Clear();
                InfoView.Columns.Add("Key", "KEY");
                InfoView.Columns.Add("IdOrder", "IdOrder");
                InfoView.Columns.Add("AmountBase", "AmountBase");
                InfoView.Columns.Add("AmountQuote", "AmountQuote");
                InfoView.Columns.Add("PricePerCoin", "PricePerCoin");
                InfoView.Columns.Add("Type", "Type");
                CurrencyPair cp;
                var trades =  PC.Trading.GetOpenOrdersAllAsync();
               // MyOpenOrder("");
                foreach (var trall in trades.Result)
                {
                    foreach (var trsin in trall.Value)
                    {
                        InfoView.Rows.Add(trall.Key, trsin.IdOrder, trsin.AmountBase, trsin.AmountQuote, trsin.PricePerCoin, trsin.Type);
                    }
                
                }



            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string qwer = "qwertyuiop";
            MessageBox.Show(qwer[1].ToString());
        }

        void LoadParams(string XMLFile)
        {
            XDocument setting;
            if (File.Exists(XMLFile))
            {
                setting = XDocument.Load(XMLFile);
                //login = setting.Root.Element("User").Value;
                //pasword = Crypt.Decrypt(setting.Root.Element("Pass").Value);
                //timemorn = setting.Root.Element("time1").Value;
                //timeeven = setting.Root.Element("time2").Value;
                //selectBrowser = int.Parse(setting.Root.Element("brows").Value);

                //ld = new List<DailyTimer>();
                //d = new DailyTimer();
                //d1 = new DailyTimer();
                //try
                //{
                //    d.T1 = TimeSpan.Parse(timemorn);
                //    d.TB = d.T1.Subtract(TimeSpan.Parse(DateTime.Now.ToShortTimeString().ToString())) > TimeSpan.Zero ? true : false;
                //    d1.T1 = TimeSpan.Parse(timeeven);
                //    d1.TB = d1.T1.Subtract(TimeSpan.Parse(DateTime.Now.ToShortTimeString().ToString())) > TimeSpan.Zero ? true : false;
                //}
                //catch { }
                //ld.Add(d);
                //ld.Add(d1);
                //timeNow.Text = "time: " + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
                //label4.Text = "time next: " + (d.TB ? d.T1.ToString() : d1.TB ? d1.T1.ToString() : "Not today");

            }
            else
            {
                setting = new XDocument(
                    new XElement("Root",
                    new XElement("User", " "),
                    new XElement("Pass", " "),
                    new XElement("time1", "08:00 "),
                    new XElement("time2", "17:00 "),
                    new XElement("brows", "0")

                ));
               

                setting.Save(XMLFile);
                MessageBox.Show("не заполнен файл bt.xml");
            }
        }

        public async void makeordertest(CurrencyPair c, double pr)
        {
            var makeord = await PC.Trading.PostOrderAsync(c, OrderType.Buy, pr, 0.0039);
        }

        public void scaling(CurrencyPair c , int period,double sum,decimal prof )
        {

            #region 
            InfoView.Rows.Clear();
            InfoView.Columns.Clear();
            InfoView.Columns.Add("Key", "KEY");
            InfoView.Columns.Add("time", "time");
            InfoView.Columns.Add("open", "open");
            InfoView.Columns.Add("close", "close");
            InfoView.Columns.Add("VolumeBase", "VolumeBase");

            #endregion

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            bool fl = true;
            
            IList<IMarketChartData> er;
                                                                //perSc
                                                                //12-9
            er = PC.Markets.GetChartDataAsync(c, (MarketPeriod)GraphsPer(period), DateTime.Now.AddHours(-5), DateTime.Now).Result;
            double price = 0;
            foreach (var ms in er.OrderByDescending(k => k.Time).Take(4))
            {
                InfoView.Rows.Add(c,ms.Time,ms.Open,ms.Close,ms.VolumeBase);
                if (ms.Open<ms.Close )
                  //  InfoView.Rows.Add(c, ms.Time, ms.Open, ms.Close, ms.VolumeBase);
               
                {
                    fl = false;
                }              
            }

            var pr = PC.Markets.GetSummaryAsync().Result[c].PriceLast;
            InfoView.Rows.Add(c,0,0, pr,0);
            makeordertest(c,pr);


            if (fl)
            {
                // only for btc,eth,xmr,usdt
                testFlag = false;
              //  var makeord =  PC.Trading.PostOrderAsync(c, OrderType.Sell, pr, sum);
                Loger.SetLog("new order on " +er.Last().Time +" " + er.Last().Close);


            }
            sw.Stop();
            TimeControls.Text = "Time: " + sw.ElapsedMilliseconds / 100.0;

        }
        bool testFlag = true;
        private void scal_Click(object sender, EventArgs e)
        {
            CurrencyPair cp = CurrencyPair.Parse("BTC_ETH");//(BaseCur.Text + "_" + QuoteCur.Text);

          //  textBox6.Text = 

            double sum = 0.0003914;
            scaling(cp, perSca.SelectedIndex, sum,0.001m);





            }
        }
}

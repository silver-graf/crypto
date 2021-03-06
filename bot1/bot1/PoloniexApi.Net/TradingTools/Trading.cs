﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Jojatekok.PoloniexAPI.TradingTools
{
    public class Trading : ITrading
    {
        private ApiWebClient ApiWebClient { get; set; }

        internal Trading(ApiWebClient apiWebClient)
        {
            ApiWebClient = apiWebClient;
        }

        public void Tst ()
        {
            IOrder a = new Order();
        }

        private IList<Order> GetOpenOrders(CurrencyPair currencyPair)
        {
            var postData = new Dictionary<string, object> {
                { "currencyPair", currencyPair }
            };

            var data = PostData<IList<Order>>("returnOpenOrders", postData);
            //IList<IOrder> tmp = (IList<IOrder>)data;
            return data; //(IList<Order>) 
        }
        private IDictionary<CurrencyPair, IList<Order>>  GetOpenOrdersAll()
        {
            var postData = new Dictionary<string, object> {
                { "currencyPair", "all" }
            };

            var data = PostData<IDictionary<string, IList<Order>>>("returnOpenOrders", postData);            
            return data.ToDictionary(
                x => CurrencyPair.Parse(x.Key),
                x => (IList<Order>)x.Value
            );
        }

        //private IList<Order> GetOpenOrdersAll(CurrencyPair currencyPair)
        //{
        //    var postData = new Dictionary<string, object> {
        //        { "currencyPair", "all" }
        //    };

        //    var data = PostData<IList<Order>>("returnOpenOrders", postData);

        //    //IList<IOrder> tmp = (IList<IOrder>)data;

        //    return data; //(IList<Order>) 
        //}


        private IList<ITrade> GetTrades(CurrencyPair currencyPair, DateTime startTime, DateTime endTime)
        {
            var postData = new Dictionary<string, object> {
                { "currencyPair", currencyPair },
                { "start", Helper.DateTimeToUnixTimeStamp(startTime) },
                { "end", Helper.DateTimeToUnixTimeStamp(endTime) }
            };

            var data = PostData<IList<Trade>>("returnTradeHistory", postData);
            return (IList<ITrade>)data;
        }

        private ulong PostOrder(CurrencyPair currencyPair, OrderType type, double pricePerCoin, double amountQuote)
        {
            var postData = new Dictionary<string, object> {
                { "currencyPair", currencyPair },
                { "rate", pricePerCoin.ToStringNormalized() },
                { "amount", amountQuote.ToStringNormalized() }
            };

            var data = PostData<JObject>(type.ToStringNormalized(), postData);
            return data.Value<ulong>("orderNumber");
        }

        private bool DeleteOrder(CurrencyPair currencyPair, ulong orderId)
        {
            var postData = new Dictionary<string, object> {
                { "currencyPair", currencyPair },
                { "orderNumber", orderId }
            };

            var data = PostData<JObject>("cancelOrder", postData);
            return data.Value<byte>("success") == 1;
        }
        public Task<IDictionary<CurrencyPair, IList<Order>>> GetOpenOrdersAllAsync()
        {
            return Task.Factory.StartNew(() => GetOpenOrdersAll());
        }

        public Task<IList<Order>> GetOpenOrdersAsync(CurrencyPair currencyPair)
        {
            return Task.Factory.StartNew(() => GetOpenOrders(currencyPair));
        }

        public Task<IList<ITrade>> GetTradesAsync(CurrencyPair currencyPair, DateTime startTime, DateTime endTime)
        {
            return Task.Factory.StartNew(() => GetTrades(currencyPair, startTime, endTime));
        }

        public Task<IList<ITrade>> GetTradesAsync(CurrencyPair currencyPair)
        {
            return Task.Factory.StartNew(() => GetTrades(currencyPair, Helper.DateTimeUnixEpochStart, DateTime.MaxValue));
        }

        public Task<ulong> PostOrderAsync(CurrencyPair currencyPair, OrderType type, double pricePerCoin, double amountQuote)
        {
            return Task.Factory.StartNew(() => PostOrder(currencyPair, type, pricePerCoin, amountQuote));
        }

        public Task<bool> DeleteOrderAsync(CurrencyPair currencyPair, ulong orderId)
        {
            return Task.Factory.StartNew(() => DeleteOrder(currencyPair, orderId));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T PostData<T>(string command, Dictionary<string, object> postData)
        {
            return ApiWebClient.PostData<T>(command, postData);
        }
    }
}

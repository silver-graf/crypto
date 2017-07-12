using Newtonsoft.Json;

namespace Jojatekok.PoloniexAPI.WalletTools
{
    public class Balance : IBalance
    {
        [JsonProperty("available")]
        public double QuoteAvailable { get; internal set; }
        [JsonProperty("onOrders")]
        public double QuoteOnOrders { get; internal set; }
        [JsonProperty("btcValue")]
        public double BitcoinValue { get; internal set; }
    }
}

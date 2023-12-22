using AngkorWat.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    internal class Active
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("ticker")]
        public string Ticker { get; set; } = string.Empty;
        public Active() { }
    }

    internal class New
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;
        [JsonProperty("rate")]
        public int Rate { get; set; }
        [JsonProperty("companiesAffected")]
        public List<string> AffectedActives { get; set; } = new();
        public New() { }
    }

    internal class AggregatedNew
    {
        public string Ticker { get; set; } = string.Empty;
        public double Rate { get; set; } = 0.0d;
        public AggregatedNew() { }
    }

    internal class Account
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    }

    internal enum BidType
    {
        BUY,
        SELL,
    }

    internal class Bid
    {
        [JsonProperty("id")]
        public int BidId { get; set; }
        [JsonProperty("account")]
        public Account Account { get; set; } = new();
        [JsonProperty("symbolId")]
        public int SymbolId { get; set; }
        [JsonProperty("price")]
        public int Price { get; set; }
        [JsonProperty("type")]
        public BidType BidType { get; set; }
        [JsonProperty("createDate")]
        public DateTime Date { get; set; }
    }

    internal class Asset
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }

    internal class AccountInfo
    {
        [JsonProperty("account")]
        public Account Account { get; set; } = new();
        [JsonProperty("bids")]
        public List<Bid> Bids { get; set; } = new();
        [JsonProperty("assets")]
        public List<Asset> Assets { get; set; } = new();
        [JsonProperty("frozenAssets")]
        public List<Asset> FrozenAssets { get; set; } = new();
    }



    internal class BidWithPriceLimit
    {
        [JsonProperty("symbolId")]
        public int SymbolId { get; set; }
        [JsonProperty("price")]
        public int Price { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
        public BidWithPriceLimit() { }
    }

    internal class BidWithoutPriceLimit
    {
        [JsonProperty("symbolId")]
        public int SymbolId { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
        public BidWithoutPriceLimit() { }
    }
}

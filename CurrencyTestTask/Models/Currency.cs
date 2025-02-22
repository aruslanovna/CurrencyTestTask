using System.Collections.Generic;

namespace CurrencyExchange.Models
{
    //public class Currency
    //{
    //    public decimal OriginalValue { get; set; }
    //    public decimal? ConvertedValue { get; set; }
    //    public CurrencyEnum CurrencyFrom { get; set; }
    //    public CurrencyEnum CurrencyTo { get; set; }
    //    public decimal ConversionRate { get; set; }
    //}

    public class Currency
    {
        public decimal OriginalValue { get; set; }
        public string CurrencyFrom { get; set; }
        public string CurrencyTo { get; set; }
        public decimal? ConvertedValue { get; set; }
        public List<string> Currencies { get; set; } = new List<string>();
    }


    public enum CurrencyEnum
    {
        USD,
        EUR,
        GBP,
        NOK
    }

    public class CurrencyResponse
    {
        public bool Success { get; set; }
        public string Terms { get; set; }
        public string Privacy { get; set; }
        public long Timestamp { get; set; }
        public string Source { get; set; }
        public Dictionary<string, decimal> Quotes { get; set; }
    }
}

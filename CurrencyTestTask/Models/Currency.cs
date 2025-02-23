using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net;

namespace CurrencyExchange.Models
{
    public class Currency
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal OriginalValue { get; set; }

        [Required]
        public string CurrencyFrom { get; set; }

        [Required]
        public string CurrencyTo { get; set; }
        public string Message { get; set; }

        public decimal? ConvertedValue { get; set; }

        public List<string> Currencies { get; set; } = new List<string>();

        public DateTime? ConversionDate { get; set; }
    }

    public class CurrencyResponse
    {
        public bool Success { get; set; }
        public string Terms { get; set; }
        public string Privacy { get; set; }
        public long Timestamp { get; set; }
        public string Source { get; set; }
        public string Base { get; set; }
        public bool Historical { get; set; } = false;
        public DateTime? Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}


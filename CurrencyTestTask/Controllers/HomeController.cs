using CurrencyExchange.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;


namespace CurrencyTestTask.Controllers
{
    public class HomeController : Controller
    {
        private HttpClient _httpClient = new HttpClient();
        private readonly string url;
        public HomeController()
        {
            url = "https://data.fixer.io/api/";
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var model = new Currency();

            List<string> currencyList = await GetCurrencies();
            model.Currencies = currencyList;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConvertCurrency(Currency model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            if (model.OriginalValue <= 0)
            {
                model.Message = "Amount must be greater than zero.";
                ModelState.AddModelError("", "Amount must be greater than zero.");
                model.ConvertedValue = 0;
            }
            else if (model.CurrencyFrom == model.CurrencyTo)
            {
                model.ConvertedValue = model.OriginalValue;
            }
            if (model.ConversionDate != null)
            {
                var currencyFrom = await GetCurrencyRate(model.CurrencyFrom, model.ConversionDate);
                var currencyTo = await GetCurrencyRate(model.CurrencyTo, model.ConversionDate);
                model.ConvertedValue = GetConvertedValue(currencyFrom, currencyTo, model.OriginalValue);
            }
            else
            {
                var currencyFrom = await GetCurrencyRate(model.CurrencyFrom);
                var currencyTo = await GetCurrencyRate(model.CurrencyTo);
                model.ConvertedValue = GetConvertedValue(currencyFrom, currencyTo, model.OriginalValue);
            }

            List<string> currencyList = await GetCurrencies();
            model.Currencies = currencyList;

            return View("Index", model);
        }

        private decimal GetConvertedValue(decimal convertFromRate, decimal convertToRate, decimal amountToConvert)
        {
            var currencyRate = (amountToConvert * convertToRate) / convertFromRate;

            return currencyRate;
        }

        private async Task<CurrencyResponse> GetResponse(DateTime? date = null)
        {
            try
            {
                var apiKey = GetApiKey();
                Uri requestUrl;

                await Task.Delay(500);

                var queryParams = new Dictionary<string, string>
                    {
                        { "access_key", apiKey }
                    };
                if (date != null)
                {
                    requestUrl = url.UriCombine(String.Format("{0:yyyy-MM-dd}", date));
                }
                else
                {
                    requestUrl = url.UriCombine("latest");
                }
                string fullUrl = requestUrl + ToQueryString(queryParams);

                string json = await _httpClient.GetStringAsync(fullUrl);

                if (string.IsNullOrWhiteSpace(json))
                {
                    throw new Exception("API response was empty.");
                }

                var currencyResponse = JsonConvert.DeserializeObject<CurrencyResponse>(json);

                if (currencyResponse == null || !currencyResponse.Success)
                {
                    throw new Exception("API request failed or exceeded rate limits.");
                }

                return currencyResponse;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error fetching currency data: " + ex.Message);
                return null;
            }
        }

        private string ToQueryString(Dictionary<string, string> requestParams)
        {
            var paramsAsString = "?" + string.Join("&", requestParams.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
            return paramsAsString;
        }

        private async Task<decimal> GetCurrencyRate(string currencyName, DateTime? conversionDate = null)
        {
            try
            {
                if (currencyName == "EUR")
                {
                    return 1;
                }

                var currencyResponse = await GetResponse(conversionDate);
                var rate = currencyResponse.Rates.Where(x => x.Key.Equals(currencyName)).Select(x => x.Value).First();

                return rate;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to find currency rate for" + currencyName + '\n' + ex);
                return 0;
            }
        }

        private async Task<List<string>> GetCurrencies()
        {
            var currencyResponse = await GetResponse();

            if (currencyResponse?.Rates != null)
            {
                var currencies = new List<string>();
                currencies.Add(currencyResponse.Base);
                foreach (var key in currencyResponse.Rates.Keys)
                {
                    string currencyCode = key;
                    currencies.Add(currencyCode);
                }

                return currencies;
            }
            return null;
        }

        private string GetApiKey()
        {
            return ConfigurationManager.AppSettings["CurrencyApiKey"];
        }
    }
}
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

        public HomeController()
        {

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

        private async Task<CurrencyResponse> GetResponse()
        {
            string apiKey = GetApiKey();
            await Task.Delay(1000);
            string json = await _httpClient.GetStringAsync($"http://api.currencylayer.com/live?access_key={apiKey}");

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

        private async Task<decimal> GetCurrencyRate(string currencyName)
        {
            try
            {
                if (currencyName == "USD")
                {
                    return 1;
                }

                var currencyResponse = await GetResponse();
                var rate = currencyResponse.Quotes.Where(x => x.Key.EndsWith(currencyName)).Select(x => x.Value).First();

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

            if (currencyResponse?.Quotes != null)
            {
                var currencies = new List<string>();
                currencies.Add(currencyResponse.Source);
                foreach (var key in currencyResponse.Quotes.Keys)
                {
                    string currencyCode = key.Substring(3);
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
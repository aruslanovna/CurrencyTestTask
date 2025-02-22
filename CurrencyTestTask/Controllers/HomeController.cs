using CurrencyExchange.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CurrencyTestTask.Controllers
{
    public class HomeController : Controller
    {
        private HttpClient _httpClient = new HttpClient();

        public HomeController()
        {

        }

        public async Task<ActionResult> Index()
        {
            var model = new Currency();

            string json = await _httpClient.GetStringAsync("http://api.currencylayer.com/live?access_key=a3d1c1eb6d68f89ee4bbfe957eeb0a9b");
            var currencyResponse = JsonConvert.DeserializeObject<CurrencyResponse>(json);

            if (currencyResponse?.Quotes != null)
            {
                var currencies = new HashSet<string>();

                foreach (var key in currencyResponse.Quotes.Keys)
                {
                    string currencyCode = key.Substring(3);
                    currencies.Add(currencyCode);
                }

                model.Currencies = new List<string>(currencies);
            }

            return View(model);
        }

        public CurrencyResponse MapToCurrencyResponse(object data)
        {
            var currencyResponse = new CurrencyResponse()
            {

            };
            return currencyResponse;
        }
    }
}
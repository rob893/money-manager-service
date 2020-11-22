using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using Microsoft.Extensions.Options;
using MoneyManagerService.Models.Settings;
using Newtonsoft.Json;
using MoneyManagerService.Entities;
using System.Linq;

namespace MoneyManagerService.Services
{
    public class AlphaVantageService
    {
        private readonly HttpClient httpClient;
        private readonly AlphaVantageSettings settings;

        public AlphaVantageService(HttpClient httpClient, IOptions<AlphaVantageSettings> settings)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));

            this.httpClient.BaseAddress = new Uri(this.settings.BaseUrl);
        }

        public async Task<IEnumerable<TickerTimeSeries>> GetDailyAdjustedTimeSeries(string ticker)
        {
            using var res = await httpClient.GetAsync($"/query?function=TIME_SERIES_DAILY_ADJUSTED&symbol={ticker}&outputsize=full&apikey={settings.ApiKey}");
            var resContent = await res.Content.ReadAsStringAsync();
            var content = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, dynamic>>>(resContent);

            var timeSeries = content["Time Series (Daily)"];

            return timeSeries.Select(entry => new TickerTimeSeries
            {
                Ticker = ticker,
                Date = DateTime.Parse(entry.Key),
                Open = double.Parse(entry.Value["1. open"].Value),
                High = double.Parse(entry.Value["2. high"].Value),
                Low = double.Parse(entry.Value["3. low"].Value),
                Close = double.Parse(entry.Value["4. close"].Value),
                AdjustedClose = double.Parse(entry.Value["5. adjusted close"].Value),
                Volume = double.Parse(entry.Value["6. volume"].Value),
                DividendAmount = double.Parse(entry.Value["7. dividend amount"].Value)
            });
        }
    }
}
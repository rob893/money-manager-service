using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using Microsoft.Extensions.Options;
using MoneyManagerService.Models.Settings;
using System.Text.Json;
using Newtonsoft.Json;

namespace MoneyManagerService.Services
{
    public class AlphaVantageService
    {
        private readonly HttpClient httpClient;
        private readonly AlphaVantageSettings settings;

        public AlphaVantageService(HttpClient httpClient, IOptions<AlphaVantageSettings> settings)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.settings = settings.Value ?? throw new ArgumentNullException(nameof(AlphaVantageSettings));

            this.httpClient.BaseAddress = new Uri(this.settings.BaseUrl);
        }

        public async Task<List<dynamic>> GetDailyAdjustedTimeSeries(string ticker)
        {
            using var res = await httpClient.GetAsync($"/query?function=TIME_SERIES_DAILY_ADJUSTED&symbol={ticker}&apikey={settings.ApiKey}");
            var resStream = await res.Content.ReadAsStringAsync();
            var content = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(resStream);

            var timeSeries = content["Time Series (Daily)"];

            var things = new List<dynamic>();

            foreach (var entry in timeSeries)
            {
                things.Add(new
                {
                    date = entry.Key
                });
            }

            //             Object.keys(timeSeries).forEach(key => {
            //     things.push({
            //         date: new Date(key),
            //         open: Number(timeSeries[key]['1. open']),
            //         close: Number(timeSeries[key]['4. close']),
            //         change: 0,
            //         dividend: Number(timeSeries[key]['7. dividend amount'])
            //     });
            // });

            return things;
        }
    }
}
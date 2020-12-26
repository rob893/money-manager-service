using System.Threading.Tasks;
using System;
using System.Net.Http;
using Microsoft.Extensions.Options;
using MoneyManagerService.Models.Settings;
using Newtonsoft.Json;
using System.Text;
using MoneyManagerService.Models.Responses.Taxee;
using Newtonsoft.Json.Serialization;
using MoneyManagerService.Models.DTOs;

namespace MoneyManagerService.Services
{
    public class TaxeeService
    {
        private readonly HttpClient httpClient;
        private readonly TaxeeSettings settings;

        public TaxeeService(HttpClient httpClient, IOptions<TaxeeSettings> settings)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));

            this.httpClient.BaseAddress = new Uri(this.settings.BaseUrl);
            this.httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {this.settings.ApiKey}");
        }

        public async Task<CalculateIncomeTaxResponse> GetIncomeTaxEstimate(CalculateIncomeTaxDto request)
        {
            var json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            });
            using var payload = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            using var res = await httpClient.PostAsync($"calculate/{request.Year}", payload);
            var resStream = await res.Content.ReadAsStringAsync();
            var content = JsonConvert.DeserializeObject<CalculateIncomeTaxResponse>(resStream, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            });

            if (content == null)
            {
                throw new NullReferenceException(nameof(content));
            }

            return content;
        }
    }
}
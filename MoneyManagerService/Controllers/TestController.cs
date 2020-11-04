using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MoneyManagerService.Services;
using System.Threading.Tasks;

namespace MoneyManagerService.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly AlphaVantageService alphaVantageService;

        public TestController(AlphaVantageService alphaVantageService)
        {
            this.alphaVantageService = alphaVantageService;
        }

        [HttpGet]
        public async Task<ActionResult<dynamic>> GetDailyAdjustedTimeSeriesAsync([FromQuery] string ticker)
        {
            var result = await alphaVantageService.GetDailyAdjustedTimeSeries(ticker);

            return result;
        }
    }
}
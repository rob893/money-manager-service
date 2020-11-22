using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MoneyManagerService.Services;
using System.Threading.Tasks;
using MoneyManagerService.Entities;
using MoneyManagerService.Models.Responses.Taxee;
using AutoMapper;
using MoneyManagerService.Models.DTOs;

namespace MoneyManagerService.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    [ApiController]
    public class TestController : ServiceControllerBase
    {
        private readonly AlphaVantageService alphaVantageService;
        private readonly TaxeeService taxeeService;
        private readonly IMapper mapper;

        public TestController(AlphaVantageService alphaVantageService, TaxeeService taxeeService, IMapper mapper)
        {
            this.alphaVantageService = alphaVantageService;
            this.taxeeService = taxeeService;
            this.mapper = mapper;
        }

        [HttpGet("alphaVantage")]
        public async Task<ActionResult<IEnumerable<TickerTimeSeries>>> GetDailyAdjustedTimeSeriesAsync([FromQuery] string ticker)
        {
            var result = await alphaVantageService.GetDailyAdjustedTimeSeries(ticker);

            return Ok(result);
        }

        [HttpPost("taxee")]
        public async Task<ActionResult<CalculateIncomeTaxResponse>> GetIncomeTaxEstimate([FromBody] CalculateIncomeTaxDto request)
        {
            var result = await taxeeService.GetIncomeTaxEstimate(request);

            return Ok(result);
        }
    }
}
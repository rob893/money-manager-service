using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MoneyManagerService.Entities;
using AutoMapper;
using MoneyManagerService.Data.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using MoneyManagerService.Extensions;
using Microsoft.AspNetCore.Authorization;
using MoneyManagerService.Constants;
using MoneyManagerService.Models.Responses;
using MoneyManagerService.Models.QueryParameters;
using System.Collections.Generic;
using MoneyManagerService.Models.DTOs.Income;

namespace MoneyManagerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncomesController : ServiceControllerBase
    {
        private readonly IncomeRepository incomeRepository;
        private readonly BudgetRepository budgetRepository;
        private readonly IMapper mapper;

        public IncomesController(IncomeRepository incomeRepository, BudgetRepository budgetRepository, IMapper mapper)
        {
            this.incomeRepository = incomeRepository;
            this.budgetRepository = budgetRepository;
            this.mapper = mapper;
        }

        [Authorize(Policy = AuthorizationPolicyName.RequireAdminRole)]
        [HttpGet]
        public async Task<ActionResult<CursorPaginatedResponse<IncomeDto>>> GetIncomesAsync([FromQuery] CursorPaginationParameters searchParams)
        {
            var expenses = await incomeRepository.SearchAsync(searchParams);
            var paginatedResponse = CursorPaginatedResponse<IncomeDto>.CreateFrom(expenses, mapper.Map<IEnumerable<IncomeDto>>, searchParams.IncludeNodes, searchParams.IncludeEdges);

            return Ok(paginatedResponse);
        }

        [HttpGet("{id}", Name = "GetIncomeAsync")]
        public async Task<ActionResult<IncomeDto>> GetIncomeAsync(int id)
        {
            var income = await incomeRepository.GetByIdAsync(id, income => income.Budget);

            if (income == null)
            {
                return NotFound($"No Income with Id {id} found.");
            }

            if (!IsUserAuthorizedForResource(income.Budget))
            {
                return Unauthorized("You can only access your own incomes.");
            }

            var incomeForReturn = mapper.Map<IncomeDto>(income);

            return Ok(incomeForReturn);
        }

        [HttpPost]
        public async Task<ActionResult<IncomeDto>> CreateIncomeAsync([FromBody] CreateIncomeDto incomeForCreateDto)
        {
            if (incomeForCreateDto.BudgetId == null)
            {
                return BadRequest("BudgetId is required.");
            }

            var budget = await budgetRepository.GetByIdAsync(incomeForCreateDto.BudgetId.Value);

            if (budget == null)
            {
                return NotFound($"No budget with id {incomeForCreateDto.BudgetId.Value} found");
            }

            if (!IsUserAuthorizedForResource(budget))
            {
                return Unauthorized($"You are not authorized to access budget with id {incomeForCreateDto.BudgetId.Value}");
            }

            var newIncome = mapper.Map<Income>(incomeForCreateDto);
            incomeRepository.Add(newIncome);

            var saveResult = await incomeRepository.SaveAllAsync();

            if (!saveResult)
            {
                return BadRequest("Unable to create expense.");
            }

            var incomeForReturn = mapper.Map<IncomeDto>(newIncome);

            return CreatedAtRoute("GetIncomeAsync", new { id = incomeForReturn.Id }, incomeForReturn);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteIncomeAsync(int id)
        {
            var income = await incomeRepository.GetByIdAsync(id, exp => exp.Budget);

            if (income == null)
            {
                return NotFound($"No Expense with Id {id} found.");
            }

            if (!IsUserAuthorizedForResource(income.Budget))
            {
                return Unauthorized("You can only access your own expenses.");
            }

            incomeRepository.Delete(income);
            var saveResults = await incomeRepository.SaveAllAsync();

            if (!saveResults)
            {
                return BadRequest("Failed to delete the income.");
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<IncomeDto>> UpdateIncomeAsync(int id, [FromBody] JsonPatchDocument<UpdateIncomeDto> dtoPatchDoc)
        {
            if (dtoPatchDoc == null || dtoPatchDoc.Operations.Count == 0)
            {
                return BadRequest("A JSON patch document with at least 1 operation is required.");
            }

            var income = await incomeRepository.GetByIdAsync(id, exp => exp.Budget);

            if (income == null)
            {
                return NotFound($"No expense with Id {id} found.");
            }

            if (!IsUserAuthorizedForResource(income.Budget))
            {
                return Unauthorized("You can only access your own expenses.");
            }

            if (!dtoPatchDoc.IsValid(out var errors))
            {
                return BadRequest(errors);
            }

            var patchDoc = mapper.Map<JsonPatchDocument<Income>>(dtoPatchDoc);

            patchDoc.ApplyTo(income);

            await incomeRepository.SaveAllAsync();

            var incomeToReturn = mapper.Map<IncomeDto>(income);

            return Ok(incomeToReturn);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MoneyManagerService.Entities;
using AutoMapper;
using MoneyManagerService.Data.Repositories;
using MoneyManagerService.Models.DTOs.Budget;
using MoneyManagerService.Models.QueryParameters;
using MoneyManagerService.Models.Responses;
using MoneyManagerService.Models.DTOs.Expense;
using Microsoft.AspNetCore.Authorization;
using MoneyManagerService.Constants;
using Microsoft.AspNetCore.JsonPatch;
using MoneyManagerService.Extensions;
using System.Collections.Generic;
using MoneyManagerService.Models.DTOs.Income;

namespace MoneyManagerService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BudgetsController : ServiceControllerBase
    {
        private readonly BudgetRepository budgetRepository;
        private readonly IMapper mapper;

        public BudgetsController(BudgetRepository budgetRepository, IMapper mapper)
        {
            this.budgetRepository = budgetRepository;
            this.mapper = mapper;
        }

        [Authorize(Policy = AuthorizationPolicyName.RequireAdminRole)]
        [HttpGet]
        public async Task<ActionResult<CursorPaginatedResponse<BudgetForReturnDto>>> GetBudgetsAsync([FromQuery] BudgetQueryParameters searchParams)
        {
            var budgets = await budgetRepository.SearchAsync(searchParams);
            var paginatedResponse = CursorPaginatedResponse<BudgetForReturnDto>.CreateFrom(budgets, mapper.Map<IEnumerable<BudgetForReturnDto>>, searchParams.IncludeNodes, searchParams.IncludeEdges);

            return Ok(paginatedResponse);
        }

        [HttpGet("{id}", Name = "GetBudgetAsync")]
        public async Task<ActionResult<BudgetForReturnDto>> GetBudgetAsync(int id)
        {
            var result = await budgetRepository.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound($"No Budget with Id {id} found.");
            }

            if (!IsUserAuthorizedForResource(result))
            {
                return Unauthorized("You can only access your own budget.");
            }

            var budgetForReturn = mapper.Map<BudgetForReturnDto>(result);

            return Ok(budgetForReturn);
        }

        [HttpPost]
        public async Task<ActionResult<BudgetForReturnDto>> CreateBudgetAsync([FromBody] BudgetForCreateDto budgetForCreateDto)
        {
            var newBudget = mapper.Map<Budget>(budgetForCreateDto);

            newBudget.TaxLiability = new TaxLiability();

            if (!IsUserAuthorizedForResource(newBudget))
            {
                return Unauthorized("You can only create budgets for yourself.");
            }

            budgetRepository.Add(newBudget);

            var saveResult = await budgetRepository.SaveAllAsync();

            if (!saveResult)
            {
                return BadRequest("Unable to create budget.");
            }

            var budgetForReturn = mapper.Map<BudgetForReturnDto>(newBudget);

            return CreatedAtRoute("GetBudgetAsync", new { id = budgetForReturn.Id }, budgetForReturn);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBudgetAsync(int id)
        {
            var budget = await budgetRepository.GetByIdAsync(id);

            if (budget == null)
            {
                return NotFound($"No Budget with Id {id} found.");
            }

            if (!IsUserAuthorizedForResource(budget))
            {
                return Unauthorized("You can only access your own budget.");
            }

            budgetRepository.Delete(budget);
            var saveResults = await budgetRepository.SaveAllAsync();

            if (!saveResults)
            {
                return BadRequest("Failed to delete the budget.");
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<BudgetForReturnDto>> UpdateBudgetAsync(int id, [FromBody] JsonPatchDocument<BudgetForUpdateDto> dtoPatchDoc)
        {
            if (dtoPatchDoc == null || dtoPatchDoc.Operations.Count == 0)
            {
                return BadRequest("A JSON patch document with at least 1 operation is required.");
            }

            var budget = await budgetRepository.GetByIdAsync(id);

            if (budget == null)
            {
                return NotFound($"No Budget with Id {id} found.");
            }

            if (!IsUserAuthorizedForResource(budget))
            {
                return Unauthorized("You can only access your own budget.");
            }

            if (!dtoPatchDoc.IsValid(out var errors))
            {
                return BadRequest(errors);
            }

            var patchDoc = mapper.Map<JsonPatchDocument<Budget>>(dtoPatchDoc);

            patchDoc.ApplyTo(budget);

            await budgetRepository.SaveAllAsync();

            var budgetForReturn = mapper.Map<BudgetForReturnDto>(budget);

            return Ok(budgetForReturn);
        }

        [HttpGet("{id}/expenses")]
        public async Task<ActionResult<CursorPaginatedResponse<ExpenseForReturnDto>>> GetExpensesForBudgetAsync(int id, [FromQuery] CursorPaginationParameters searchParams)
        {
            var budget = await budgetRepository.GetByIdAsync(id);

            if (budget == null)
            {
                return NotFound($"No Budget with Id {id} found.");
            }

            if (!IsUserAuthorizedForResource(budget))
            {
                return Unauthorized("You can only access your own budget.");
            }

            var expenses = await budgetRepository.GetExpensesForBudgetAsync(id, searchParams);
            var paginatedResponse = CursorPaginatedResponse<ExpenseForReturnDto>.CreateFrom(expenses, mapper.Map<IEnumerable<ExpenseForReturnDto>>, searchParams.IncludeNodes, searchParams.IncludeEdges);

            return Ok(paginatedResponse);
        }

        [HttpPost("{id}/expenses")]
        public async Task<ActionResult<ExpenseForReturnDto>> CreateExpenseForBudgetAsync(int id, [FromBody] ExpenseForCreateDto expenseForCreateDto)
        {
            var budget = await budgetRepository.GetByIdAsync(id);

            if (budget == null)
            {
                return NotFound($"No Budget with id {id} found.");
            }

            if (!IsUserAuthorizedForResource(budget))
            {
                return Unauthorized("You can only access your own budget.");
            }

            var newExpense = mapper.Map<Expense>(expenseForCreateDto);
            newExpense.BudgetId = id;

            budget.Expenses.Add(newExpense);

            var saveResult = await budgetRepository.SaveAllAsync();

            if (!saveResult)
            {
                return BadRequest("Unable to create expense.");
            }

            var expenseToReturn = mapper.Map<ExpenseForReturnDto>(newExpense);

            return CreatedAtRoute("GetExpenseAsync", new { id = expenseToReturn.Id }, expenseToReturn);
        }

        [HttpPost("{id}/incomes")]
        public async Task<ActionResult<IncomeForReturnDto>> CreateIncomeForBudgetAsync([FromRoute] int id, [FromBody] IncomeForCreateDto incomeForCreateDto)
        {
            var budget = await budgetRepository.GetByIdAsync(id);

            if (budget == null)
            {
                return NotFound($"No Budget with id {id} found.");
            }

            if (!IsUserAuthorizedForResource(budget))
            {
                return Unauthorized("You can only access your own budget.");
            }

            var newIncome = mapper.Map<Income>(incomeForCreateDto);
            newIncome.BudgetId = id;

            budget.Incomes.Add(newIncome);

            var saveResult = await budgetRepository.SaveAllAsync();

            if (!saveResult)
            {
                return BadRequest("Unable to create income.");
            }

            var incomeToReturn = mapper.Map<IncomeForReturnDto>(newIncome);

            return CreatedAtRoute("GetExpenseAsync", new { id = incomeToReturn.Id }, incomeToReturn);
        }
    }
}
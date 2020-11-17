using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MoneyManagerService.Models.Domain;
using AutoMapper;
using MoneyManagerService.Data.Repositories;
using MoneyManagerService.Models.DTOs.Budget;
using MoneyManagerService.Models.QueryParameters;
using MoneyManagerService.Models.Responses;
using MoneyManagerService.Models.DTOs.Expense;
using Microsoft.AspNetCore.Authorization;
using MoneyManagerService.Constants;
using Microsoft.AspNetCore.JsonPatch;

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
        public async Task<ActionResult<CursorPaginatedResponse<Budget>>> GetBudgetsAsync([FromQuery] CursorPaginationParameters searchParams)
        {
            var budgets = await budgetRepository.SearchAsync(searchParams);
            var paginatedResponse = CursorPaginatedResponse<Budget>.CreateFrom(budgets, searchParams.IncludeNodes, searchParams.IncludeEdges);

            return Ok(paginatedResponse);
        }

        [HttpGet("{id}", Name = "GetBudgetAsync")]
        public async Task<ActionResult<Budget>> GetBudgetAsync(int id)
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

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Budget>> CreateBudgetAsync([FromBody] BudgetForCreateDto budgetForCreateDto)
        {
            var newBudget = mapper.Map<Budget>(budgetForCreateDto);
            budgetRepository.Add(newBudget);

            var saveResult = await budgetRepository.SaveAllAsync();

            if (!saveResult)
            {
                return BadRequest("Unable to create budget.");
            }

            return CreatedAtRoute("GetBudgetAsync", new { id = newBudget.Id }, newBudget);
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
        public async Task<ActionResult<Budget>> UpdateBudgetAsync(int id, [FromBody] JsonPatchDocument<Budget> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
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

            patchDoc.ApplyTo(budget);

            var saveResult = await budgetRepository.SaveAllAsync();

            if (!saveResult)
            {
                return BadRequest("Could not apply changes.");
            }

            return Ok(budget);
        }

        [HttpGet("{id}/expenses")]
        public async Task<ActionResult<CursorPaginatedResponse<Expense>>> GetExpensesForBudgetAsync(int id, [FromQuery] CursorPaginationParameters searchParams)
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
            var paginatedResponse = CursorPaginatedResponse<Expense>.CreateFrom(expenses, searchParams.IncludeNodes, searchParams.IncludeEdges);

            return Ok(paginatedResponse);
        }

        [HttpPost("{id}/expenses")]
        public async Task<ActionResult<Expense>> CreateExpenseForBudgetAsync(int id, [FromBody] ExpenseForCreateDto expenseForCreateDto)
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

            return CreatedAtRoute("GetExpenseAsync", new { id = newExpense.Id }, newExpense);
        }
    }
}
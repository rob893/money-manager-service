using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MoneyManagerService.Models.Domain;
using AutoMapper;
using MoneyManagerService.Data.Repositories;
using MoneyManagerService.Models.DTOs.Budget;
using MoneyManagerService.Models.QueryParameters;
using MoneyManagerService.Models.Responses;
using MoneyManagerService.Models.DTOs.Expense;
using MoneyManagerService.Core;

namespace MoneyManagerService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BudgetsController : ServiceControllerBase
    {
        private readonly BudgetRepository budgetRepository;
        private readonly ExpenseRepository expenseRepository;
        private readonly IMapper mapper;

        public BudgetsController(BudgetRepository budgetRepository, ExpenseRepository expenseRepository, IMapper mapper)
        {
            this.budgetRepository = budgetRepository;
            this.expenseRepository = expenseRepository;
            this.mapper = mapper;
        }

        [HttpGet("{id}", Name = "GetBudgetAsync")]
        public async Task<ActionResult<Budget>> GetBudgetAsync(int id)
        {
            var result = await budgetRepository.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound($"No Budget with Id {id} found.");
            }

            if (!User.IsAdmin() && (!User.TryGetUserId(out int userId) || result.UserId != userId))
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

            budgetRepository.Delete(budget);
            var saveResults = await budgetRepository.SaveAllAsync();

            if (!saveResults)
            {
                return BadRequest("Failed to delete the budget.");
            }

            return NoContent();
        }

        [HttpGet("{id}/expenses")]
        public async Task<ActionResult<CursorPaginatedResponse<Expense>>> GeAsync(int id, [FromQuery] CursorPaginationParameters searchParams)
        {
            var expenses = await budgetRepository.GetExpensesForBudgetAsync(id, searchParams);
            var paginatedResponse = CursorPaginatedResponse<Expense>.CreateFrom(expenses);

            return Ok(paginatedResponse);
        }

        [HttpPost("{budgetId}/expenses")]
        public async Task<ActionResult<Expense>> CreateExpenseForBudgetAsync(int budgetId, [FromBody] ExpenseForCreateDto expenseForCreateDto)
        {
            var budget = await budgetRepository.GetByIdAsync(budgetId);

            if (budget == null)
            {
                return NotFound($"No Budget with id {budgetId} found.");
            }

            var newExpense = mapper.Map<Expense>(expenseForCreateDto);
            newExpense.BudgetId = budgetId;

            budget.Expenses.Add(newExpense);
            // expenseRepository.Add(newExpense);

            var saveResult = await expenseRepository.SaveAllAsync();

            if (!saveResult)
            {
                return BadRequest("Unable to create expense.");
            }

            return CreatedAtRoute("GetExpenseAsync", new { id = newExpense.Id }, newExpense);
        }
    }
}
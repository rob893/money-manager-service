using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MoneyManagerService.Models.Domain;
using AutoMapper;
using MoneyManagerService.Data.Repositories;
using MoneyManagerService.Models.DTOs.Expense;
using Microsoft.AspNetCore.JsonPatch;
using MoneyManagerService.Extensions;
using Microsoft.AspNetCore.Authorization;
using MoneyManagerService.Constants;
using MoneyManagerService.Models.Responses;
using MoneyManagerService.Models.QueryParameters;
using System.Collections.Generic;

namespace MoneyManagerService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExpensesController : ServiceControllerBase
    {
        private readonly ExpenseRepository expenseRepository;
        private readonly BudgetRepository budgetRepository;
        private readonly IMapper mapper;

        public ExpensesController(ExpenseRepository expenseRepository, BudgetRepository budgetRepository, IMapper mapper)
        {
            this.expenseRepository = expenseRepository;
            this.budgetRepository = budgetRepository;
            this.mapper = mapper;
        }

        [Authorize(Policy = AuthorizationPolicyName.RequireAdminRole)]
        [HttpGet]
        public async Task<ActionResult<CursorPaginatedResponse<ExpenseForReturnDto>>> GetExpensesAsync([FromQuery] CursorPaginationParameters searchParams)
        {
            var expenses = await expenseRepository.SearchAsync(searchParams);
            var paginatedResponse = CursorPaginatedResponse<ExpenseForReturnDto>.CreateFrom(expenses, mapper.Map<IEnumerable<ExpenseForReturnDto>>, searchParams.IncludeNodes, searchParams.IncludeEdges);

            return Ok(paginatedResponse);
        }

        [HttpGet("{id}", Name = "GetExpenseAsync")]
        public async Task<ActionResult<ExpenseForReturnDto>> GetExpenseAsync(int id)
        {
            var expense = await expenseRepository.GetByIdAsync(id, exp => exp.Budget);

            if (expense == null)
            {
                return NotFound($"No Expense with Id {id} found.");
            }

            if (!IsUserAuthorizedForResource(expense.Budget))
            {
                return Unauthorized("You can only access your own expenses.");
            }

            var expenseForReturn = mapper.Map<ExpenseForReturnDto>(expense);

            return Ok(expenseForReturn);
        }

        [HttpPost]
        public async Task<ActionResult<ExpenseForReturnDto>> CreateExpenseAsync([FromBody] ExpenseForCreateDto expenseForCreateDto)
        {
            if (expenseForCreateDto.BudgetId == null)
            {
                return BadRequest("BudgetId is required.");
            }

            var budget = await budgetRepository.GetByIdAsync(expenseForCreateDto.BudgetId.Value);

            if (budget == null)
            {
                return NotFound($"No budget with id {expenseForCreateDto.BudgetId.Value} found");
            }

            if (!IsUserAuthorizedForResource(budget))
            {
                return Unauthorized($"You are not authorized to access budget with id {expenseForCreateDto.BudgetId.Value}");
            }

            var newExpense = mapper.Map<Expense>(expenseForCreateDto);
            expenseRepository.Add(newExpense);

            var saveResult = await expenseRepository.SaveAllAsync();

            if (!saveResult)
            {
                return BadRequest("Unable to create expense.");
            }

            var expenseForReturn = mapper.Map<ExpenseForReturnDto>(newExpense);

            return CreatedAtRoute("GetExpenseAsync", new { id = expenseForReturn.Id }, expenseForReturn);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteExpenseAsync(int id)
        {
            var expense = await expenseRepository.GetByIdAsync(id, exp => exp.Budget);

            if (expense == null)
            {
                return NotFound($"No Expense with Id {id} found.");
            }

            if (!IsUserAuthorizedForResource(expense.Budget))
            {
                return Unauthorized("You can only access your own expenses.");
            }

            expenseRepository.Delete(expense);
            var saveResults = await expenseRepository.SaveAllAsync();

            if (!saveResults)
            {
                return BadRequest("Failed to delete the expense.");
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<ExpenseForReturnDto>> UpdateExpenseAsync(int id, [FromBody] JsonPatchDocument<ExpenseForUpdateDto> dtoPatchDoc)
        {
            if (dtoPatchDoc == null || dtoPatchDoc.Operations.Count == 0)
            {
                return BadRequest("A JSON patch document with at least 1 operation is required.");
            }

            var expense = await expenseRepository.GetByIdAsync(id, exp => exp.Budget);

            if (expense == null)
            {
                return NotFound($"No expense with Id {id} found.");
            }

            if (!IsUserAuthorizedForResource(expense.Budget))
            {
                return Unauthorized("You can only access your own expenses.");
            }

            if (!dtoPatchDoc.IsValid(out var errors))
            {
                return BadRequest(errors);
            }

            var patchDoc = mapper.Map<JsonPatchDocument<Expense>>(dtoPatchDoc);

            patchDoc.ApplyTo(expense);

            await expenseRepository.SaveAllAsync();

            var expenseToReturn = mapper.Map<ExpenseForReturnDto>(expense);

            return Ok(expenseToReturn);
        }
    }
}
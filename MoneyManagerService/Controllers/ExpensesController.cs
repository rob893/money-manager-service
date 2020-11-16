using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MoneyManagerService.Models.Domain;
using AutoMapper;
using MoneyManagerService.Data.Repositories;
using MoneyManagerService.Models.DTOs.Expense;

namespace MoneyManagerService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExpensesController : ServiceControllerBase
    {
        private readonly ExpenseRepository expenseRepository;
        private readonly IMapper mapper;

        public ExpensesController(ExpenseRepository expenseRepository, IMapper mapper)
        {
            this.expenseRepository = expenseRepository;
            this.mapper = mapper;
        }

        [HttpGet("{id}", Name = "GetExpenseAsync")]
        public async Task<ActionResult<Expense>> GetExpenseAsync(int id)
        {
            var result = await expenseRepository.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound($"No Expense with Id {id} found.");
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Expense>> CreateExpenseAsync([FromBody] ExpenseForCreateDto expenseForCreateDto)
        {
            var newExpense = mapper.Map<Expense>(expenseForCreateDto);
            expenseRepository.Add(newExpense);

            var saveResult = await expenseRepository.SaveAllAsync();

            if (!saveResult)
            {
                return BadRequest("Unable to create expense.");
            }

            return CreatedAtRoute("GetExpenseAsync", new { id = newExpense.Id }, newExpense);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteExpenseAsync(int id)
        {
            var expense = await expenseRepository.GetByIdAsync(id);

            if (expense == null)
            {
                return NotFound($"No Expense with Id {id} found.");
            }

            expenseRepository.Delete(expense);
            var saveResults = await expenseRepository.SaveAllAsync();

            if (!saveResults)
            {
                return BadRequest("Failed to delete the expense.");
            }

            return NoContent();
        }
    }
}
using System.Linq;
using System.Threading.Tasks;
using MoneyManagerService.Core;
using MoneyManagerService.Models.Domain;
using MoneyManagerService.Models.QueryParameters;

namespace MoneyManagerService.Data.Repositories
{
    public class BudgetRepository : Repository<Budget, CursorPaginationParameters>
    {
        public BudgetRepository(DataContext context) : base(context) { }

        public Task<CursorPagedList<Expense, int>> GetExpensesForBudgetAsync(int budgetId, CursorPaginationParameters searchParams)
        {
            IQueryable<Expense> query = context.Expenses
                .Where(expense => expense.BudgetId == budgetId);

            return CursorPagedList<Expense, int>.CreateAsync(query, searchParams);
        }
    }
}
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoneyManagerService.Core;
using MoneyManagerService.Entities;
using MoneyManagerService.Models.QueryParameters;

namespace MoneyManagerService.Data.Repositories
{
    public class BudgetRepository : Repository<Budget, BudgetQueryParameters>
    {
        public BudgetRepository(DataContext context) : base(context) { }

        public Task<CursorPagedList<Expense, int>> GetExpensesForBudgetAsync(int budgetId, CursorPaginationParameters searchParams)
        {
            IQueryable<Expense> query = context.Expenses
                .Where(expense => expense.BudgetId == budgetId)
                .Include(ex => ex.Tags);

            return CursorPagedList<Expense, int>.CreateAsync(query, searchParams);
        }

        protected override IQueryable<Budget> AddWhereClauses(IQueryable<Budget> query, BudgetQueryParameters searchParams)
        {
            if (searchParams.UserIds != null && searchParams.UserIds.Count > 0)
            {
                query = query.Where(budget => searchParams.UserIds.Contains(budget.UserId));
            }

            return query;
        }

        protected override IQueryable<Budget> AddIncludes(IQueryable<Budget> query)
        {
            return query
                .Include(budget => budget.TaxLiability)
                .Include(budget => budget.Expenses)
                .Include(budget => budget.Incomes);
        }
    }
}
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MoneyManagerService.Entities;
using MoneyManagerService.Models.QueryParameters;

namespace MoneyManagerService.Data.Repositories
{
    public class ExpenseRepository : Repository<Expense, CursorPaginationParameters>
    {
        public ExpenseRepository(DataContext context) : base(context) { }

        protected override IQueryable<Expense> AddIncludes(IQueryable<Expense> query)
        {
            return query.Include(expense => expense.Tags);
        }
    }
}
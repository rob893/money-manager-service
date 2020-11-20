using System.Collections.Generic;

namespace MoneyManagerService.Models.Domain
{
    public class Budget : IIdentifiable<int>, IOwnedByUser<int>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public List<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
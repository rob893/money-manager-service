using System.Collections.Generic;

namespace MoneyManagerService.Entities
{
    public class Tag : IIdentifiable<int>, IOwnedByUser<int>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = default!;
        public string Name { get; set; } = default!;
        public List<Expense> Expenses { get; set; } = new();
    }
}
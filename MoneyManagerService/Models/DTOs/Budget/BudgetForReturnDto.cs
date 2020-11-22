using MoneyManagerService.Models.Domain;

namespace MoneyManagerService.Models.DTOs.Budget
{
    public record BudgetForReturnDto : IIdentifiable<int>, IOwnedByUser<int>
    {
        public int Id { get; init; }
        public int UserId { get; init; }
        public string Name { get; init; } = default!;
        public string? Description { get; init; }
    }
}
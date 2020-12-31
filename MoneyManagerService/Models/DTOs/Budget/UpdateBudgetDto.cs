namespace MoneyManagerService.Models.DTOs.Budget
{
    public record UpdateBudgetDto
    {
        public string? Name { get; init; }
        public string? Description { get; init; }
    }
}
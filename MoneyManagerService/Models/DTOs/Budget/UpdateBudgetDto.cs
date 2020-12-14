namespace MoneyManagerService.Models.DTOs.Budget
{
    public record UpdateBudgetDto
    {
        public string? Description { get; init; }
    }
}
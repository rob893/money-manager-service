using MoneyManagerService.Entities;

namespace MoneyManagerService.Models.DTOs.Tag
{
    public record TagDto : IIdentifiable<int>
    {
        public int Id { get; init; }
        public int UserId { get; init; } = default!;
        public string Name { get; init; } = default!;
    }
}
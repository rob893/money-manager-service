using MoneyManagerService.Models.Domain;

namespace MoneyManagerService.Models.DTOs
{
    public record RoleForReturnDto : IIdentifiable<int>
    {
        public int Id { get; init; }
        public string Name { get; init; } = default!;
        public string NormalizedName { get; init; } = default!;
    }
}
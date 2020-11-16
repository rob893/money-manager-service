using MoneyManagerService.Models.Domain;

namespace MoneyManagerService.Models.DTOs
{
    public record RoleForReturnDto : IIdentifiable<int>
    {
        public int Id { get; set; }
        public string Name { get; init; }
        public string NormalizedName { get; init; }
    }
}
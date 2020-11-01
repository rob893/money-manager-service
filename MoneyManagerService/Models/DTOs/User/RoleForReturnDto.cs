using MoneyManagerService.Models.Domain;

namespace MoneyManagerService.Models.DTOs
{
    public class RoleForReturnDto : IIdentifiable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
    }
}
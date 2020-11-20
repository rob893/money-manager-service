using Microsoft.AspNetCore.Identity;

namespace MoneyManagerService.Models.Domain
{
    public class UserRole : IdentityUserRole<int>
    {
        public User User { get; set; } = default!;
        public Role Role { get; set; } = default!;
    }
}
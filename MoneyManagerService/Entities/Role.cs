using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace MoneyManagerService.Entities
{
    public class Role : IdentityRole<int>, IIdentifiable<int>
    {
        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
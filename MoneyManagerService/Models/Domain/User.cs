using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MoneyManagerService.Models.Domain
{
    public class User : IdentityUser<int>, IIdentifiable<int>
    {
        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }
        public DateTimeOffset Created { get; set; }
        public RefreshToken RefreshToken { get; set; }
        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public List<Budget> Budgets { get; set; } = new List<Budget>();
    }
}
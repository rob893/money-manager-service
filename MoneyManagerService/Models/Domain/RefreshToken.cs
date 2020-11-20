using System;
using System.ComponentModel.DataAnnotations;

namespace MoneyManagerService.Models.Domain
{
    public class RefreshToken : IOwnedByUser<int>
    {
        public int UserId { get; set; }
        public User User { get; set; } = default!;
        [Required]
        [MaxLength(255)]
        public string Token { get; set; } = default!;
        public DateTimeOffset Expiration { get; set; }
    }
}
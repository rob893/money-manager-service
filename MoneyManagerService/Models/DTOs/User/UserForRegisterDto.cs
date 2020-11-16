using System;
using System.ComponentModel.DataAnnotations;

namespace MoneyManagerService.Models.DTOs
{
    public record UserForRegisterDto
    {
        [Required]
        public string UserName { get; init; }

        [Required]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "You must specify a password between 4 and 10 characters")]
        public string Password { get; init; }

        [Required]
        [MaxLength(255)]
        public string FirstName { get; init; }

        [Required]
        [MaxLength(255)]
        public string LastName { get; init; }

        [Required]
        public string Email { get; init; }

        public DateTimeOffset Created { get; init; }


        public UserForRegisterDto()
        {
            Created = DateTimeOffset.UtcNow;
        }
    }
}
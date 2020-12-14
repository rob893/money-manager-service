using System;
using System.ComponentModel.DataAnnotations;

namespace MoneyManagerService.Models.DTOs
{
    public record RegisterUserDto
    {
        [Required]
        public string UserName { get; init; } = default!;

        [Required]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "You must specify a password between 4 and 10 characters")]
        public string Password { get; init; } = default!;

        [Required]
        [MaxLength(255)]
        public string FirstName { get; init; } = default!;

        [Required]
        [MaxLength(255)]
        public string LastName { get; init; } = default!;

        [Required]
        public string Email { get; init; } = default!;

        public DateTimeOffset Created { get; init; }


        public RegisterUserDto()
        {
            Created = DateTimeOffset.UtcNow;
        }
    }
}
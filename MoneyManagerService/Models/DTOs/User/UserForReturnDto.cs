using System;
using System.Collections.Generic;
using MoneyManagerService.Entities;

namespace MoneyManagerService.Models.DTOs
{
    public record UserForReturnDto : IIdentifiable<int>
    {
        public int Id { get; init; }
        public string UserName { get; init; } = default!;
        public string FirstName { get; init; } = default!;
        public string LastName { get; init; } = default!;
        public string Email { get; init; } = default!;
        public DateTimeOffset Created { get; init; }
        public IEnumerable<string> Roles { get; init; } = new List<string>();
    }
}
using System;
using System.Collections.Generic;
using MoneyManagerService.Models.Domain;

namespace MoneyManagerService.Models.DTOs
{
    public record UserForReturnDto : IIdentifiable<int>
    {
        public int Id { get; set; }
        public string UserName { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public DateTimeOffset Created { get; init; }
        public IEnumerable<string> Roles { get; init; }
    }
}
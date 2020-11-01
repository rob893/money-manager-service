using System;
using System.Collections.Generic;
using MoneyManagerService.Models.Domain;

namespace MoneyManagerService.Models.DTOs
{
    public class UserForReturnDto : IIdentifiable
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTimeOffset Created { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
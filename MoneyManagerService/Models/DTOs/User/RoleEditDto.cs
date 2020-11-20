using System;

namespace MoneyManagerService.Models.DTOs
{
    public record RoleEditDto
    {
        public string[] RoleNames { get; init; } = Array.Empty<string>();
    }
}
using System.ComponentModel.DataAnnotations;

namespace MoneyManagerService.Models.DTOs
{
    public record CalculateIncomeTaxDto
    {
        [Required]
        public string Year { get; init; } = default!;
        [Required]
        public double PayRate { get; init; }
        [Required]
        public string FilingStatus { get; init; } = default!;
        [Required]
        public string State { get; init; } = default!;
        public int? PayPeriods { get; init; }
        public int? Exemptions { get; init; }
    }
}
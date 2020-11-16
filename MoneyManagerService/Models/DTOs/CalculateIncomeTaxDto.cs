using System.ComponentModel.DataAnnotations;

namespace MoneyManagerService.Models.DTOs
{
    public record CalculateIncomeTaxDto
    {
        [Required]
        public string Year { get; init; }
        [Required]
        public double PayRate { get; init; }
        [Required]
        public string FilingStatus { get; init; }
        [Required]
        public string State { get; init; }
        public int? PayPeriods { get; init; }
        public int? Exemptions { get; init; }
    }
}
namespace NetPay.DataProcessor.ImportDtos
{
    using System.ComponentModel.DataAnnotations;

    public class ImportExpenseDto
    {
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string ExpenseName { get; set; } = null!;

        [Required]
        [Range(0.01, 100000)]
        public decimal Amount { get; set; }

        [Required]
        public string DueDate { get; set; } = null!;

        [Required]
        public string PaymentStatus { get; set; } = null!;

        [Required]
        public int HouseholdId { get; set; }

        [Required]
        public int ServiceId { get; set; }
    }
}

namespace TravelAgency.DataProcessor.ImportDtos
{
    using System.ComponentModel.DataAnnotations;

    public class ImportBookingDto
    {
        [Required]
        public string BookingDate { get; set; } = null!;

        public string? CustomerName { get; set; }

        public string? TourPackageName { get; set; }
    }
}

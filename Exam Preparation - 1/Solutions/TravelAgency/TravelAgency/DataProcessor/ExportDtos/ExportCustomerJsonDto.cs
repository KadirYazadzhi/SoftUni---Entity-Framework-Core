namespace TravelAgency.DataProcessor.ExportDtos
{
    public class ExportCustomerJsonDto
    {
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public ExportBookingJsonDto[] Bookings { get; set; } = null!;
    }

    public class ExportBookingJsonDto
    {
        public string TourPackageName { get; set; } = null!;
        public string Date { get; set; } = null!;
    }
}

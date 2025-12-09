namespace TravelAgency.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;
    using Newtonsoft.Json;
    using TravelAgency.Data;
    using TravelAgency.Data.Models;
    using TravelAgency.DataProcessor.ImportDtos;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data format!";
        private const string DuplicationDataMessage = "Error! Data duplicated.";
        private const string SuccessfullyImportedCustomer = "Successfully imported customer - {0}";
        private const string SuccessfullyImportedBooking = "Successfully imported booking. TourPackage: {0}, Date: {1}";

        public static string ImportCustomers(TravelAgencyContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(ImportCustomerDto[]), new XmlRootAttribute("Customers"));
            using var reader = new StringReader(xmlString);
            var customerDtos = (ImportCustomerDto[])serializer.Deserialize(reader)!;

            var customers = new List<Customer>();

            foreach (var dto in customerDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (customers.Any(c => c.FullName == dto.FullName || c.Email == dto.Email || c.PhoneNumber == dto.PhoneNumber) ||
                    context.Customers.Any(c => c.FullName == dto.FullName || c.Email == dto.Email || c.PhoneNumber == dto.PhoneNumber))
                {
                    sb.AppendLine(DuplicationDataMessage);
                    continue;
                }

                var customer = new Customer
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber
                };

                customers.Add(customer);
                sb.AppendLine(string.Format(SuccessfullyImportedCustomer, customer.FullName));
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportBookings(TravelAgencyContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var bookingDtos = JsonConvert.DeserializeObject<ImportBookingDto[]>(jsonString)!;
            var bookings = new List<Booking>();

            foreach (var dto in bookingDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime bookingDate;
                if (!DateTime.TryParseExact(dto.BookingDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out bookingDate))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var customer = context.Customers.FirstOrDefault(c => c.FullName == dto.CustomerName);
                var tourPackage = context.TourPackages.FirstOrDefault(tp => tp.PackageName == dto.TourPackageName);

                if (customer == null || tourPackage == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var booking = new Booking
                {
                    BookingDate = bookingDate,
                    Customer = customer,
                    TourPackage = tourPackage
                };

                bookings.Add(booking);
                sb.AppendLine(string.Format(SuccessfullyImportedBooking, tourPackage.PackageName, bookingDate.ToString("yyyy-MM-dd")));
            }

            context.Bookings.AddRange(bookings);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static bool IsValid(object dto)
        {
            var validateContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(dto, validateContext, validationResults, true);

            foreach (var validationResult in validationResults)
            {
                string currValidationMessage = validationResult.ErrorMessage;
            }

            return isValid;
        }
    }
}

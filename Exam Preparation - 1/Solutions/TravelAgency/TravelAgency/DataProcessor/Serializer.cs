namespace TravelAgency.DataProcessor
{
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;
    using Newtonsoft.Json;
    using TravelAgency.Data;
    using TravelAgency.Data.Models.Enums;
    using TravelAgency.DataProcessor.ExportDtos;

    public class Serializer
    {
        public static string ExportGuidesWithSpanishLanguageWithAllTheirTourPackages(TravelAgencyContext context)
        {
            var guides = context.Guides
                .Where(g => g.Language == Language.Spanish)
                .OrderByDescending(g => g.TourPackagesGuides.Count)
                .ThenBy(g => g.FullName)
                .Select(g => new ExportGuideDto
                {
                    FullName = g.FullName,
                    TourPackages = g.TourPackagesGuides
                        .Select(tpg => tpg.TourPackage)
                        .OrderByDescending(tp => tp.Price)
                        .ThenBy(tp => tp.PackageName)
                        .Select(tp => new ExportTourPackageDto
                        {
                            Name = tp.PackageName,
                            Description = tp.Description,
                            Price = tp.Price
                        })
                        .ToArray()
                })
                .ToArray();

            var serializer = new XmlSerializer(typeof(ExportGuideDto[]), new XmlRootAttribute("Guides"));
            var sb = new StringBuilder();
            using var writer = new StringWriter(sb);
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");
            serializer.Serialize(writer, guides, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string ExportCustomersThatHaveBookedHorseRidingTourPackage(TravelAgencyContext context)
        {
            var customers = context.Customers
                .Where(c => c.Bookings.Any(b => b.TourPackage.PackageName == "Horse Riding Tour"))
                .Select(c => new ExportCustomerJsonDto
                {
                    FullName = c.FullName,
                    PhoneNumber = c.PhoneNumber,
                    Bookings = c.Bookings
                        .Where(b => b.TourPackage.PackageName == "Horse Riding Tour")
                        .OrderBy(b => b.BookingDate)
                        .Select(b => new ExportBookingJsonDto
                        {
                            TourPackageName = b.TourPackage.PackageName,
                            Date = b.BookingDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                        })
                        .ToArray()
                })
                .OrderByDescending(c => c.Bookings.Length)
                .ThenBy(c => c.FullName)
                .ToArray();

            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }
    }
}

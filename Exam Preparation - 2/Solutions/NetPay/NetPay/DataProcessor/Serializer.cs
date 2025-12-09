namespace NetPay.DataProcessor
{
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;
    using Newtonsoft.Json;
    using NetPay.Data;
    using NetPay.Data.Models.Enums;
    using NetPay.DataProcessor.ExportDtos;

    public class Serializer
    {
        public static string ExportHouseholdsWhichHaveExpensesToPay(NetPayContext context)
        {
            var households = context.Households
                .Where(h => h.Expenses.Any(e => e.PaymentStatus != PaymentStatus.Paid))
                .OrderBy(h => h.ContactPerson)
                .Select(h => new ExportHouseholdDto
                {
                    ContactPerson = h.ContactPerson,
                    Email = h.Email,
                    PhoneNumber = h.PhoneNumber,
                    Expenses = h.Expenses
                        .Where(e => e.PaymentStatus != PaymentStatus.Paid)
                        .OrderBy(e => e.DueDate)
                        .ThenBy(e => e.Amount)
                        .Select(e => new ExportExpenseDto
                        {
                            ExpenseName = e.ExpenseName,
                            Amount = e.Amount.ToString("F2"),
                            PaymentDate = e.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                            ServiceName = e.Service.ServiceName
                        })
                        .ToArray()
                })
                .ToArray();

            var serializer = new XmlSerializer(typeof(ExportHouseholdDto[]), new XmlRootAttribute("Households"));
            var sb = new StringBuilder();
            using var writer = new StringWriter(sb);
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");
            serializer.Serialize(writer, households, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string ExportAllServicesWithSuppliers(NetPayContext context)
        {
            var services = context.Services
                .OrderBy(s => s.ServiceName)
                .Select(s => new ExportServiceDto
                {
                    ServiceName = s.ServiceName,
                    Suppliers = s.SuppliersServices
                        .Select(ss => ss.Supplier)
                        .OrderBy(sup => sup.SupplierName)
                        .Select(sup => new ExportSupplierDto
                        {
                            SupplierName = sup.SupplierName
                        })
                        .ToArray()
                })
                .ToArray();

            return JsonConvert.SerializeObject(services, Formatting.Indented);
        }
    }
}

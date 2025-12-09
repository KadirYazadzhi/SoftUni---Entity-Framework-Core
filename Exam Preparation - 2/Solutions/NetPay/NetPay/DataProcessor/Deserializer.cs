namespace NetPay.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;
    using Newtonsoft.Json;
    using NetPay.Data;
    using NetPay.Data.Models;
    using NetPay.Data.Models.Enums;
    using NetPay.DataProcessor.ImportDtos;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data format!";
        private const string DuplicationDataMessage = "Error! Data duplicated.";
        private const string SuccessfullyImportedHousehold = "Successfully imported household. Contact person: {0}";
        private const string SuccessfullyImportedExpense = "Successfully imported expense. {0}, Amount: {1}";

        public static string ImportHouseholds(NetPayContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(ImportHouseholdDto[]), new XmlRootAttribute("Households"));
            using var reader = new StringReader(xmlString);
            var householdDtos = (ImportHouseholdDto[])serializer.Deserialize(reader)!;

            var households = new List<Household>();

            foreach (var dto in householdDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (households.Any(h => h.ContactPerson == dto.ContactPerson || h.Email == dto.Email || h.PhoneNumber == dto.PhoneNumber) ||
                    context.Households.Any(h => h.ContactPerson == dto.ContactPerson || h.Email == dto.Email || h.PhoneNumber == dto.PhoneNumber))
                {
                    sb.AppendLine(DuplicationDataMessage);
                    continue;
                }

                var household = new Household
                {
                    ContactPerson = dto.ContactPerson,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber
                };

                households.Add(household);
                sb.AppendLine(string.Format(SuccessfullyImportedHousehold, household.ContactPerson));
            }

            context.Households.AddRange(households);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportExpenses(NetPayContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var expenseDtos = JsonConvert.DeserializeObject<ImportExpenseDto[]>(jsonString)!;
            var expenses = new List<Expense>();

            foreach (var dto in expenseDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!Enum.TryParse(dto.PaymentStatus, out PaymentStatus paymentStatus))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var household = context.Households.Find(dto.HouseholdId);
                var service = context.Services.Find(dto.ServiceId);

                if (household == null || service == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime dueDate;
                if (!DateTime.TryParseExact(dto.DueDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dueDate))
                {
                     // Some formats in JSON might have time, let's try generic parse or fix format
                     if(!DateTime.TryParse(dto.DueDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out dueDate))
                     {
                         sb.AppendLine(ErrorMessage);
                         continue;
                     }
                }

                var expense = new Expense
                {
                    ExpenseName = dto.ExpenseName,
                    Amount = dto.Amount,
                    DueDate = dueDate,
                    PaymentStatus = paymentStatus,
                    Household = household,
                    Service = service
                };

                expenses.Add(expense);
                sb.AppendLine(string.Format(SuccessfullyImportedExpense, expense.ExpenseName, expense.Amount.ToString("F2")));
            }

            context.Expenses.AddRange(expenses);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            foreach (var result in validationResults)
            {
                string currvValidationMessage = result.ErrorMessage;
            }

            return isValid;
        }
    }
}

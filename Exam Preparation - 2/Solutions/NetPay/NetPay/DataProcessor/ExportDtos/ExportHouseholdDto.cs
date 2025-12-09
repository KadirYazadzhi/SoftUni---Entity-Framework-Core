namespace NetPay.DataProcessor.ExportDtos
{
    using System.Xml.Serialization;

    [XmlType("Household")]
    public class ExportHouseholdDto
    {
        [XmlElement("ContactPerson")]
        public string ContactPerson { get; set; } = null!;

        [XmlElement("Email")]
        public string? Email { get; set; }

        [XmlElement("PhoneNumber")]
        public string PhoneNumber { get; set; } = null!;

        [XmlArray("Expenses")]
        [XmlArrayItem("Expense")]
        public ExportExpenseDto[] Expenses { get; set; } = null!;
    }

    [XmlType("Expense")]
    public class ExportExpenseDto
    {
        [XmlElement("ExpenseName")]
        public string ExpenseName { get; set; } = null!;

        [XmlElement("Amount")]
        public string Amount { get; set; } = null!; // Formatted string

        [XmlElement("PaymentDate")]
        public string PaymentDate { get; set; } = null!;

        [XmlElement("ServiceName")]
        public string ServiceName { get; set; } = null!;
    }
}

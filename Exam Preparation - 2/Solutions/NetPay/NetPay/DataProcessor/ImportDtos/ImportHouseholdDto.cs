namespace NetPay.DataProcessor.ImportDtos
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Household")]
    public class ImportHouseholdDto
    {
        [XmlAttribute("phone")]
        [Required]
        [StringLength(15)]
        [RegularExpression(@"^\+\d{3}/\d{3}-\d{6}$")]
        public string PhoneNumber { get; set; } = null!;

        [XmlElement("ContactPerson")]
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string ContactPerson { get; set; } = null!;

        [XmlElement("Email")]
        [StringLength(80, MinimumLength = 6)]
        public string? Email { get; set; }
    }
}

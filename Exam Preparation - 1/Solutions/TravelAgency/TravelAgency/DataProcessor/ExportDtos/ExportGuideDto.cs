namespace TravelAgency.DataProcessor.ExportDtos
{
    using System.Xml.Serialization;

    [XmlType("Guide")]
    public class ExportGuideDto
    {
        [XmlElement("FullName")]
        public string FullName { get; set; } = null!;

        [XmlArray("TourPackages")]
        [XmlArrayItem("TourPackage")]
        public ExportTourPackageDto[] TourPackages { get; set; } = null!;
    }

    [XmlType("TourPackage")]
    public class ExportTourPackageDto
    {
        [XmlElement("Name")]
        public string Name { get; set; } = null!;

        [XmlElement("Description")]
        public string? Description { get; set; }

        [XmlElement("Price")]
        public decimal Price { get; set; }
    }
}

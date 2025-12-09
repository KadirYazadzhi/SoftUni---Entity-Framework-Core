namespace CarDealer.DTOs.Import {
    using System.Xml.Serialization;

    [XmlType("Car")]
    public class ImportCarDto {
        [XmlElement("make")]
        public string Make { get; set; } = null!;

        [XmlElement("model")]
        public string Model { get; set; } = null!;

        [XmlElement("traveledDistance")]
        public long TraveledDistance { get; set; }

        [XmlArray("parts")]
        [XmlArrayItem("partId")]
        public ImportCarPartDto[] Parts { get; set; } = null!;
    }

    [XmlType("partId")]
    public class ImportCarPartDto {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}

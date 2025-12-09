namespace ProductShop.DTOs.Export {
    using System.Xml.Serialization;

    [XmlType("User")]
    public class ExportUserSoldProductsDto {
        [XmlElement("firstName")]
        public string? FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; } = null!;

        [XmlArray("soldProducts")]
        [XmlArrayItem("Product")]
        public ExportProductDto[] SoldProducts { get; set; } = null!;
    }

    [XmlType("Product")]
    public class ExportProductDto {
        [XmlElement("name")]
        public string Name { get; set; } = null!;

        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}

namespace ProductShop.DTOs.Export {
    using System.Xml.Serialization;

    [XmlRoot("Users")]
    public class ExportUsersRootDto {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("users")]
        [XmlArrayItem("User")]
        public ExportUserWithProductsDto[] Users { get; set; } = null!;
    }

    [XmlType("User")]
    public class ExportUserWithProductsDto {
        [XmlElement("firstName")]
        public string? FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; } = null!;

        [XmlElement("age")]
        public int? Age { get; set; }

        [XmlElement("SoldProducts")]
        public ExportSoldProductsDto SoldProducts { get; set; } = null!;
    }

    public class ExportSoldProductsDto {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("products")]
        [XmlArrayItem("Product")]
        public ExportProductSimpleDto[] Products { get; set; } = null!;
    }

    [XmlType("Product")]
    public class ExportProductSimpleDto {
        [XmlElement("name")]
        public string Name { get; set; } = null!;

        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}

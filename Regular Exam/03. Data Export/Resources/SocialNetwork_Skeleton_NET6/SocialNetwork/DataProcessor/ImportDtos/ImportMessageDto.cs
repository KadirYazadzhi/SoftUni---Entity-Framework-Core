using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace SocialNetwork.DataProcessor.ImportDtos
{
    [XmlType("Message")]
    public class ImportMessageDto
    {
        [XmlElement("Content")]
        [Required]
        [MinLength(1)]
        [MaxLength(200)]
        public string Content { get; set; } = null!;

        [XmlAttribute("SentAt")]
        [Required]
        public string SentAt { get; set; } = null!;

        [XmlElement("Status")]
        [Required]
        public string Status { get; set; } = null!;

        [XmlElement("ConversationId")]
        [Required]
        public int ConversationId { get; set; }

        [XmlElement("SenderId")]
        [Required]
        public int SenderId { get; set; }
    }
}

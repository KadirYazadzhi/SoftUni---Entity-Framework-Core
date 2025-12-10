using Newtonsoft.Json;
using SocialNetwork.Data;
using SocialNetwork.Data.Models;
using SocialNetwork.Data.Models.Enums;
using SocialNetwork.DataProcessor.ImportDtos;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace SocialNetwork.DataProcessor
{
    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data format.";
        private const string DuplicatedDataMessage = "Duplicated data.";
        private const string SuccessfullyImportedMessageEntity = "Successfully imported message (Sent at: {0}, Status: {1})";
        private const string SuccessfullyImportedPostEntity = "Successfully imported post (Creator {0}, Created at: {1})";

        public static string ImportMessages(SocialNetworkDbContext dbContext, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportMessageDto[]), new XmlRootAttribute("Messages"));

            using StringReader reader = new StringReader(xmlString);
            ImportMessageDto[] messageDtos = (ImportMessageDto[])xmlSerializer.Deserialize(reader)!;

            ICollection<Message> validMessages = new HashSet<Message>();

            foreach (var dto in messageDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool isSentAtValid = DateTime.TryParseExact(dto.SentAt, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime sentAt);
                if (!isSentAtValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                // Check length again if needed, but IsValid handles annotations.
                // Status valid enum value?
                bool isStatusValid = Enum.TryParse<MessageStatus>(dto.Status, out MessageStatus status);
                if (!isStatusValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                // Check IDs exist
                var conversation = dbContext.Conversations.Find(dto.ConversationId);
                var sender = dbContext.Users.Find(dto.SenderId);

                if (conversation == null || sender == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                // Duplication check
                // Content, SentAt, Status, and SenderId within the same ConversationId
                bool isDuplicate = dbContext.Messages.Any(m => m.Content == dto.Content &&
                                                               m.SentAt == sentAt &&
                                                               m.Status == status &&
                                                               m.SenderId == dto.SenderId &&
                                                               m.ConversationId == dto.ConversationId);
                
                // Also check against messages already in validMessages list to avoid duplicates in the same batch
                bool isDuplicateInBatch = validMessages.Any(m => m.Content == dto.Content &&
                                                               m.SentAt == sentAt &&
                                                               m.Status == status &&
                                                               m.SenderId == dto.SenderId &&
                                                               m.ConversationId == dto.ConversationId);

                if (isDuplicate || isDuplicateInBatch)
                {
                    sb.AppendLine(DuplicatedDataMessage);
                    continue;
                }

                Message message = new Message()
                {
                    Content = dto.Content,
                    SentAt = sentAt,
                    Status = status,
                    ConversationId = dto.ConversationId,
                    SenderId = dto.SenderId
                };

                validMessages.Add(message);
                sb.AppendLine(string.Format(SuccessfullyImportedMessageEntity, sentAt.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture), status.ToString()));
            }

            dbContext.Messages.AddRange(validMessages);
            dbContext.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPosts(SocialNetworkDbContext dbContext, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            ImportPostDto[] postDtos = JsonConvert.DeserializeObject<ImportPostDto[]>(jsonString)!;

            ICollection<Post> validPosts = new HashSet<Post>();

            foreach (var dto in postDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool isCreatedAtValid = DateTime.TryParseExact(dto.CreatedAt, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime createdAt);
                if (!isCreatedAtValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var creator = dbContext.Users.Find(dto.CreatorId);
                if (creator == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                // Duplication: Content, CreatedAt, and CreatorId
                bool isDuplicate = dbContext.Posts.Any(p => p.Content == dto.Content &&
                                                            p.CreatedAt == createdAt &&
                                                            p.CreatorId == dto.CreatorId);
                
                bool isDuplicateInBatch = validPosts.Any(p => p.Content == dto.Content &&
                                                            p.CreatedAt == createdAt &&
                                                            p.CreatorId == dto.CreatorId);

                if (isDuplicate || isDuplicateInBatch)
                {
                    sb.AppendLine(DuplicatedDataMessage);
                    continue;
                }

                Post post = new Post()
                {
                    Content = dto.Content,
                    CreatedAt = createdAt,
                    CreatorId = dto.CreatorId
                };

                validPosts.Add(post);
                sb.AppendLine(string.Format(SuccessfullyImportedPostEntity, creator.Username, createdAt.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture)));
            }

            dbContext.Posts.AddRange(validPosts);
            dbContext.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static bool IsValid(object dto)
        {
            ValidationContext validationContext = new ValidationContext(dto);
            List<ValidationResult> validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            foreach (ValidationResult validationResult in validationResults)
            {
                if (validationResult.ErrorMessage != null)
                {
                    string currentMessage = validationResult.ErrorMessage;
                }
            }

            return isValid;
        }
    }
}

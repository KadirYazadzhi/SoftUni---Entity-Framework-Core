using Newtonsoft.Json;
using SocialNetwork.Data;
using SocialNetwork.DataProcessor.ExportDTOs;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace SocialNetwork.DataProcessor
{
    public class Serializer
    {
        public static string ExportUsersWithFriendShipsCountAndTheirPosts(SocialNetworkDbContext dbContext)
        {
            var usersData = dbContext.Users
                .Select(u => new
                {
                    u.Username,
                    FriendshipsCount = dbContext.Friendships.Count(f => f.UserOneId == u.Id || f.UserTwoId == u.Id),
                    Posts = u.Posts
                        .OrderBy(p => p.Id)
                        .Select(p => new
                        {
                            p.Content,
                            p.CreatedAt
                        })
                        .ToArray()
                })
                .OrderBy(u => u.Username)
                .ToArray();

            var usersDtos = usersData
                .Select(u => new ExportUserDto
                {
                    Username = u.Username,
                    Friendships = u.FriendshipsCount,
                    Posts = u.Posts.Select(p => new ExportPostDto
                    {
                        Content = p.Content,
                        CreatedAt = p.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture)
                    }).ToArray()
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportUserDto[]), new XmlRootAttribute("Users"));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using (StringWriter writer = new StringWriter(sb))
            {
                xmlSerializer.Serialize(writer, usersDtos, namespaces);
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportConversationsWithMessagesChronologically(SocialNetworkDbContext dbContext)
        {
            var conversations = dbContext.Conversations
                .OrderBy(c => c.StartedAt)
                .Select(c => new
                {
                    c.Id,
                    c.Title,
                    c.StartedAt,
                    Messages = c.Messages.OrderBy(m => m.SentAt).Select(m => new
                    {
                        m.Content,
                        m.SentAt,
                        m.Status,
                        SenderUsername = m.Sender.Username
                    })
                })
                .ToList();

            var result = conversations.Select(c => new
            {
                c.Id,
                c.Title,
                StartedAt = c.StartedAt.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture),
                Messages = c.Messages.Select(m => new
                {
                    m.Content,
                    SentAt = m.SentAt.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture),
                    Status = (int)m.Status,
                    m.SenderUsername
                })
            }).ToList();

            return JsonConvert.SerializeObject(result, Formatting.Indented);
        }
    }
}

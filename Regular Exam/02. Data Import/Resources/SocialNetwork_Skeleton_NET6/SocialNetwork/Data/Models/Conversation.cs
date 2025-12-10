using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Data.Models
{
    public class Conversation
    {
        public Conversation()
        {
            this.Messages = new HashSet<Message>();
            this.UsersConversations = new HashSet<UserConversation>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Title { get; set; } = null!;

        [Required]
        public DateTime StartedAt { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<UserConversation> UsersConversations { get; set; }
    }
}

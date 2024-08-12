using System;
using System.ComponentModel.DataAnnotations;

namespace MatchingApp.Models.Entities
{
    public class Message
    {
        [Key]
        public int Id { get; set; } // Unique identifier for each message

        public int SenderId { get; set; } // ID of the user sending the message

        public int ReceiverId { get; set; } // ID of the user receiving the message

        [Required]
        [MaxLength(500)]
        public string Content { get; set; } // Content of the message

        public DateTime DateSent { get; set; } // Date and time when the message was sent

        public bool IsRead { get; set; } // Flag indicating whether the message has been read
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace MatchingApp.Models.Entities
{
    public class Match
    {
        [Key]
        public int Id { get; set; } // Unique identifier for each match

        public int LikerId { get; set; } // ID of the user who did the liking

        public int LikedId { get; set; } // ID of the user who was liked

        public bool IsMutual { get; set; } // Flag indicating if the like was mutual

        public DateTime DateLiked { get; set; } // Date and time when the like occurred
    }
}

using System.ComponentModel.DataAnnotations;

namespace MatchingApp.Models.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; } // Id

        public string Gender { get; set; } // Gender

        public int Age { get; set; } // Age

        public decimal Credits { get; set; } // Credits

        public bool Active { get; set; } // Active
    }
}
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DiscussionService.Models
{
    // This class represents the difficulty level discussion ID for a map. This is a child discussion of a map discussion.
    public class Discussion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [NotMapped]
        public string HexId => Id.ToString("X");

        public int Characteristic { get; set; }
        public int Difficulty { get; set; }

        public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;

        public bool IsLocked { get; set; }
    }
}

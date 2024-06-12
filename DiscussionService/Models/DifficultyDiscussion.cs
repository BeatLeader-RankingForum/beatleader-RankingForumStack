using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DiscussionService.Models
{
    // This class represents the difficulty level discussion ID for a map. This is a child discussion of a map discussion.
    public class DifficultyDiscussion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        [ForeignKey(nameof(MapDiscussion))]
        public string MapDiscussionId { get; set; }

        public int Characteristic { get; set; }
        public int Difficulty { get; set; }

        public bool IsLocked { get; set; } = false;
    }

    
}

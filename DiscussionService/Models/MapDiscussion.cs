using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiscussionService.Models
{
    // This class represents the top level discussion ID for a map.
    public class MapDiscussion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required string MapsetId { get; set; } // Must be unique, this can be used as a secondary primary key for use in the front-end

        public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;

        public List<string> DiscussionOwnerIds { get; set; } = new List<string>(); // The user IDs of the accounts responsible for replying to this discussion AKA mapper(s) of the map

        public required List<Discussion> Discussions { get; set; }
    }
}

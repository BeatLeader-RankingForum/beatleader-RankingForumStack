namespace DiscussionService.Models
{
    // This class represents the top level discussion ID for a map.
    public class MapDiscussion
    {
        public Guid MapDiscussionId { get; set; }
        public required string MapId { get; set; }
        public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;

        public List<string> DiscussionOwnerIds { get; set; } = new List<string>(); // The user IDs of the accounts responsible for replying to this discussion AKA mapper(s) of the map

        public required List<Discussion> Discussions { get; set; }
    }
}

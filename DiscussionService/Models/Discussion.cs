namespace DiscussionService.Models
{
    // This class represents the difficulty level discussion ID for a map. This is a child discussion of a map discussion.
    public class Discussion
    {
        public Guid DiscussionId { get; set; }
        public required string DifficultyId { get; set; }
        public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;

        public bool IsLocked { get; set; }
        public bool IsDisabled { get; set; } // This is used to disable a discussion when the difficulty is not present in the current map version
    }
}

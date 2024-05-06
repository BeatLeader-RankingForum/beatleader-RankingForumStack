namespace Contracts
{
    public record DiscussionCreatedEvent
    {
        public int Id { get; set; }
        public required string MapsetId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}

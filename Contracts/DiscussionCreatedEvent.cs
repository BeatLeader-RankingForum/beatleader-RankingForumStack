namespace Contracts
{
    public record DiscussionCreatedEvent
    {
        public required string Id { get; set; }
        public required string MapsetId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}

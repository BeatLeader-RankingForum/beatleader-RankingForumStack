namespace Contracts
{
    public record DiscussionCreatedEvent
    {
        public Guid Id { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}

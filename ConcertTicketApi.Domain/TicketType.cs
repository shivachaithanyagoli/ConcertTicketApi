namespace ConcertTicketApi.Domain.Models
{
    public class TicketType
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Event Event { get; set; } = default!;
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public int AvailableQuantity { get; set; }
    }
}

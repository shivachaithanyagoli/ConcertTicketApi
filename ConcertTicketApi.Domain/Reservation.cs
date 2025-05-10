namespace ConcertTicketApi.Domain.Models
{
    public class Reservation
    {
        public Guid Id { get; set; }
        public Guid TicketTypeId { get; set; }
        public TicketType TicketType { get; set; } = default!;
        public string CustomerName { get; set; } = default!;
        public DateTime ReservedAt { get; set; }
        public bool IsPurchased { get; set; } = false;
    }
}
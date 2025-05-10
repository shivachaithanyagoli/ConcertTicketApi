namespace ConcertTicketApi.Domain.Models
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public DateTime Date { get; set; }
        public string Venue { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int Capacity { get; set; }
        public List<TicketType> TicketTypes { get; set; } = new();
    }
}

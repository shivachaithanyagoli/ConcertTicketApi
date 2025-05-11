using System;

namespace ConcertTicketApi.Api.Models
{
    public class TicketTypeDto
    {
        public Guid Id                { get; set; }
        public string Name            { get; set; } = string.Empty;
        public decimal Price          { get; set; }
        public int AvailableQuantity  { get; set; }

        // Optionally: include event info or just omit EventId for simplicity
        public Guid EventId           { get; set; }
    }
}

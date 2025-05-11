using System;
using System.Collections.Generic;

namespace ConcertTicketApi.Api.Models
{
    public class EventDto
    {
        public Guid Id           { get; set; }
        public string Name       { get; set; } = string.Empty;
        public DateTime Date     { get; set; }
        public string Venue      { get; set; } = string.Empty;
        public string Description{ get; set; } = string.Empty;
        public int Capacity      { get; set; }

        // Include ticket types in the event response
        public List<TicketTypeDto> TicketTypes { get; set; } = new();
    }
}

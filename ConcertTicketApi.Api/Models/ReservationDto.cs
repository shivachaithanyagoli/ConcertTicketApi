using System;

namespace ConcertTicketApi.Api.Models
{
    public class ReservationDto
    {
        public Guid Id             { get; set; }
        public Guid TicketTypeId   { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime ReservedAt { get; set; }
        public bool IsPurchased    { get; set; }
    }
}

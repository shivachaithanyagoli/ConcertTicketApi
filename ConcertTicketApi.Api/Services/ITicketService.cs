using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConcertTicketApi.Api.Models;

namespace ConcertTicketApi.Api.Services
{
    public interface ITicketService
    {
       
        Task<IEnumerable<TicketTypeDto>> GetAvailabilityAsync(Guid eventId);

       
        Task<ReservationDto> ReserveAsync(Guid eventId, Guid ticketTypeId, string customerName);

       
        Task<ReservationDto> PurchaseAsync(Guid eventId, Guid ticketTypeId, Guid reservationId);

       
        Task CancelAsync(Guid eventId, Guid ticketTypeId, Guid reservationId);
    }
}

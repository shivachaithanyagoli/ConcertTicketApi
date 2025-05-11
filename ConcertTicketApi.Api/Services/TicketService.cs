using AutoMapper;
using ConcertTicketApi.Api.Models;
using ConcertTicketApi.Domain.Models;
using ConcertTicketApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ConcertTicketApi.Api.Services
{
    public class TicketService : ITicketService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper             _mapper;

        public TicketService(ApplicationDbContext db, IMapper mapper)
        {
            _db     = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TicketTypeDto>> GetAvailabilityAsync(Guid eventId)
        {
            var ev = await _db.Events
                .Include(e => e.TicketTypes)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (ev is null) throw new KeyNotFoundException($"Event {eventId} not found");
            return _mapper.Map<IEnumerable<TicketTypeDto>>(ev.TicketTypes);
        }

        public async Task<ReservationDto> ReserveAsync(Guid eventId, Guid ticketTypeId, string customerName)
        {
            var tt = await _db.TicketTypes
                .FirstOrDefaultAsync(t => t.Id == ticketTypeId && t.EventId == eventId);

            if (tt == null || tt.AvailableQuantity < 1)
                throw new InvalidOperationException("Ticket not available");

            tt.AvailableQuantity--;
            var res = new Reservation
            {
                Id            = Guid.NewGuid(),
                TicketTypeId  = ticketTypeId,
                CustomerName  = customerName,
                ReservedAt    = DateTime.UtcNow
            };

            _db.Reservations.Add(res);
            await _db.SaveChangesAsync();

            return _mapper.Map<ReservationDto>(res);
        }

        public async Task<ReservationDto> PurchaseAsync(Guid eventId, Guid ticketTypeId, Guid reservationId)
        {
            var res = await _db.Reservations
                .Include(r => r.TicketType)
                .FirstOrDefaultAsync(r =>
                    r.Id == reservationId &&
                    r.TicketTypeId == ticketTypeId &&
                    r.TicketType.EventId == eventId);

            if (res == null) throw new KeyNotFoundException("Reservation not found");

            res.IsPurchased = true;
            await _db.SaveChangesAsync();

            return _mapper.Map<ReservationDto>(res);
        }

        public async Task CancelAsync(Guid eventId, Guid ticketTypeId, Guid reservationId)
        {
            var res = await _db.Reservations
                .Include(r => r.TicketType)
                .FirstOrDefaultAsync(r =>
                    r.Id == reservationId &&
                    r.TicketType.EventId == eventId);

            if (res == null) throw new KeyNotFoundException("Reservation not found");

            _db.Reservations.Remove(res);
            res.TicketType.AvailableQuantity++;
            await _db.SaveChangesAsync();
        }
    }
}

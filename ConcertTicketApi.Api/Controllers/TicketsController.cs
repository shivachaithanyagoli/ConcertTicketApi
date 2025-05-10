using ConcertTicketApi.Domain.Models;
using ConcertTicketApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConcertTicketApi.Api.Controllers
{
    [ApiController]
    [Route("api/events/{eventId:guid}/tickets")]
    public class TicketsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public TicketsController(ApplicationDbContext db) => _db = db;

        // GET /api/events/{eventId}/tickets/availability
        [HttpGet("availability")]
        public async Task<IActionResult> Availability(Guid eventId)
        {
            var ev = await _db.Events
                .Include(e => e.TicketTypes)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (ev == null) return NotFound();
            return Ok(ev.TicketTypes.Select(tt => new {
                tt.Id, tt.Name, tt.Price, tt.AvailableQuantity
            }));
        }

        // POST /api/events/{eventId}/tickets/{ticketTypeId}/reserve
        [HttpPost("{ticketTypeId:guid}/reserve")]
        public async Task<IActionResult> Reserve(
            Guid eventId,
            Guid ticketTypeId,
            [FromBody] string customerName)
        {
            var tt = await _db.TicketTypes
                .FirstOrDefaultAsync(x => x.Id == ticketTypeId && x.EventId == eventId);
            if (tt == null || tt.AvailableQuantity < 1)
                return BadRequest("Not available");

            tt.AvailableQuantity--;
            var res = new Reservation {
                Id = Guid.NewGuid(),
                TicketTypeId = ticketTypeId,
                CustomerName = customerName,
                ReservedAt = DateTime.UtcNow
            };
            _db.Reservations.Add(res);
            await _db.SaveChangesAsync();

            return Ok(res);
        }

        // POST /api/events/{eventId}/tickets/{ticketTypeId}/purchase/{reservationId}
        [HttpPost("{ticketTypeId:guid}/purchase/{reservationId:guid}")]
        public async Task<IActionResult> Purchase(
            Guid eventId, Guid ticketTypeId, Guid reservationId)
        {
            var res = await _db.Reservations
                .Include(r => r.TicketType)
                .FirstOrDefaultAsync(r =>
                    r.Id == reservationId &&
                    r.TicketTypeId == ticketTypeId &&
                    r.TicketType.EventId == eventId);

            if (res == null) return NotFound();
            res.IsPurchased = true;
            await _db.SaveChangesAsync();
            return Ok(res);
        }

        // DELETE /api/events/{eventId}/tickets/{ticketTypeId}/cancel/{reservationId}
        [HttpDelete("{ticketTypeId:guid}/cancel/{reservationId:guid}")]
        public async Task<IActionResult> Cancel(
            Guid eventId, Guid ticketTypeId, Guid reservationId)
        {
            var res = await _db.Reservations
                .Include(r => r.TicketType)
                .FirstOrDefaultAsync(r =>
                    r.Id == reservationId &&
                    r.TicketType.EventId == eventId);

            if (res == null) return NotFound();
            _db.Reservations.Remove(res);
            res.TicketType.AvailableQuantity++;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

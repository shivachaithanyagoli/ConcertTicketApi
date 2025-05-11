using AutoMapper;
using ConcertTicketApi.Api.Models;
using ConcertTicketApi.Api.Services;       
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConcertTicketApi.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/events/{eventId:guid}/tickets")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _svc;
        private readonly IMapper        _mapper;

        public TicketsController(ITicketService svc, IMapper mapper)
        {
            _svc    = svc;
            _mapper = mapper;
        }

        // GET availability
        [HttpGet("availability")]
        public async Task<IActionResult> Availability(Guid eventId)
        {
            var list = await _svc.GetAvailabilityAsync(eventId);
            return Ok(list);
        }

        // POST reserve
        [HttpPost("{ticketTypeId:guid}/reserve")]
        public async Task<IActionResult> Reserve(
            Guid eventId,
            Guid ticketTypeId,
            [FromBody] string customerName)
        {
            var dto = await _svc.ReserveAsync(eventId, ticketTypeId, customerName);
            return Ok(dto);
        }

        // POST purchase
        [HttpPost("{ticketTypeId:guid}/purchase/{reservationId:guid}")]
        public async Task<IActionResult> Purchase(
            Guid eventId, Guid ticketTypeId, Guid reservationId)
        {
            var dto = await _svc.PurchaseAsync(eventId, ticketTypeId, reservationId);
            return Ok(dto);
        }

        // DELETE cancel
        [HttpDelete("{ticketTypeId:guid}/cancel/{reservationId:guid}")]
        public async Task<IActionResult> Cancel(
            Guid eventId, Guid ticketTypeId, Guid reservationId)
        {
            await _svc.CancelAsync(eventId, ticketTypeId, reservationId);
            return NoContent();
        }
    }
}

using ConcertTicketApi.Api.Models;
using ConcertTicketApi.Domain.Models;
using ConcertTicketApi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConcertTicketApi.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public EventsController(ApplicationDbContext db) => _db = db;

        
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _db.Events.Include(e => e.TicketTypes).ToListAsync());

        
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id) =>
            await _db.Events.Include(e => e.TicketTypes)
                .FirstOrDefaultAsync(e => e.Id == id)
            is Event ev ? Ok(ev) : NotFound();

        
        [HttpPost]
        public async Task<IActionResult> Create(CreateEventDto dto)
        {
            var ev = new Event
            {
                Id          = Guid.NewGuid(),
                Name        = dto.Name,
                Date        = dto.Date,
                Venue       = dto.Venue,
                Description = dto.Description,
                Capacity    = dto.Capacity
            };

            _db.Events.Add(ev);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = ev.Id }, ev);
        }

        
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, CreateEventDto dto)
        {
            var ev = await _db.Events.FindAsync(id);
            if (ev == null) return NotFound();

            ev.Name        = dto.Name;
            ev.Date        = dto.Date;
            ev.Venue       = dto.Venue;
            ev.Description = dto.Description;
            ev.Capacity    = dto.Capacity;

            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

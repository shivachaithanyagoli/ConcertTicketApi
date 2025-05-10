using ConcertTicketApi.Domain.Models;
using ConcertTicketApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConcertTicketApi.Api.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public EventsController(ApplicationDbContext db) => _db = db;

        // GET /api/events
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _db.Events.Include(e => e.TicketTypes).ToListAsync());

        // GET /api/events/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id) =>
            await _db.Events.Include(e => e.TicketTypes)
                .FirstOrDefaultAsync(e => e.Id == id)
            is Event ev ? Ok(ev) : NotFound();

        // POST /api/events
        [HttpPost]
        public async Task<IActionResult> Create(Event ev)
        {
            ev.Id = Guid.NewGuid();
            _db.Events.Add(ev);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = ev.Id }, ev);
        }

        // PUT /api/events/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, Event input)
        {
            var ev = await _db.Events.FindAsync(id);
            if (ev == null) return NotFound();

            ev.Name = input.Name;
            ev.Date = input.Date;
            ev.Venue = input.Venue;
            ev.Description = input.Description;
            ev.Capacity = input.Capacity;
            // (Optionally update TicketTypes here)

            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

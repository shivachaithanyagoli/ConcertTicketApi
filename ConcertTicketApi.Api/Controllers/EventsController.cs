using ConcertTicketApi.Infrastructure;
using ConcertTicketApi.Domain.Models;
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
        public async Task<IActionResult> Create(Event ev)
        {
            ev.Id = Guid.NewGuid();
            _db.Events.Add(ev);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = ev.Id }, ev);
        }

      
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
         

            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

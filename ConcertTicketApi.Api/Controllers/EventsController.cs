using AutoMapper;
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
        private readonly IMapper              _mapper;

        public EventsController(ApplicationDbContext db, IMapper mapper)
        {
            _db     = db;
            _mapper = mapper;
        }

        // GET /api/events
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var events = await _db.Events
                .Include(e => e.TicketTypes)
                .ToListAsync();

            var dtoList = _mapper.Map<IEnumerable<EventDto>>(events);
            return Ok(dtoList);
        }

        // GET /api/events/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var ev = await _db.Events
                .Include(e => e.TicketTypes)
                .FirstOrDefaultAsync(e => e.Id == id);

            return ev is null
                ? NotFound()
                : Ok(_mapper.Map<EventDto>(ev));
        }

        // POST /api/events
        [HttpPost]
        public async Task<IActionResult> Create(CreateEventDto dto)
        {
            var ev = _mapper.Map<Event>(dto);
            ev.Id = Guid.NewGuid();

            _db.Events.Add(ev);
            await _db.SaveChangesAsync();

            var result = _mapper.Map<EventDto>(ev);
            return CreatedAtAction(nameof(Get), new { id = ev.Id }, result);
        }

        // PUT /api/events/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, CreateEventDto dto)
        {
            var ev = await _db.Events.FindAsync(id);
            if (ev == null) return NotFound();

            _mapper.Map(dto, ev);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}

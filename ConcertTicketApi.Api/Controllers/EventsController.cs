using AutoMapper;
using ConcertTicketApi.Api.Models;
using ConcertTicketApi.Api.Services;      
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConcertTicketApi.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _svc;
        private readonly IMapper       _mapper;

        public EventsController(IEventService svc, IMapper mapper)
        {
            _svc    = svc;
            _mapper = mapper;
        }

        // GET /api/events
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _svc.GetAllAsync();
            return Ok(list);
        }

        // GET /api/events/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var dto = await _svc.GetByIdAsync(id);
            return dto is null ? NotFound() : Ok(dto);
        }

        // POST /api/events
        [HttpPost]
        public async Task<IActionResult> Create(CreateEventDto dto)
        {
            var result = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        // PUT /api/events/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, CreateEventDto dto)
        {
            await _svc.UpdateAsync(id, dto);
            return NoContent();
        }
    }
}

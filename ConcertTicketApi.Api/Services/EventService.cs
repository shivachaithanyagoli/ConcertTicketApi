using AutoMapper;
using ConcertTicketApi.Api.Models;
using ConcertTicketApi.Domain.Models;
using ConcertTicketApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ConcertTicketApi.Api.Services
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper             _mapper;

        public EventService(ApplicationDbContext db, IMapper mapper)
        {
            _db     = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EventDto>> GetAllAsync()
        {
            var entities = await _db.Events
                .Include(e => e.TicketTypes)
                .ToListAsync();

            return _mapper.Map<IEnumerable<EventDto>>(entities);
        }

        public async Task<EventDto?> GetByIdAsync(Guid id)
        {
            var ev = await _db.Events
                .Include(e => e.TicketTypes)
                .FirstOrDefaultAsync(e => e.Id == id);

            return ev is null ? null : _mapper.Map<EventDto>(ev);
        }

        public async Task<EventDto> CreateAsync(CreateEventDto dto)
        {
            var ev = _mapper.Map<Event>(dto);
            ev.Id = Guid.NewGuid();

            _db.Events.Add(ev);
            await _db.SaveChangesAsync();

            return _mapper.Map<EventDto>(ev);
        }

        public async Task UpdateAsync(Guid id, CreateEventDto dto)
        {
            var ev = await _db.Events.FindAsync(id)
                     ?? throw new KeyNotFoundException($"Event {id} not found");

            _mapper.Map(dto, ev);
            await _db.SaveChangesAsync();
        }
    }
}

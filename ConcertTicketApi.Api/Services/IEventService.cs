using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConcertTicketApi.Api.Models;

namespace ConcertTicketApi.Api.Services
{
    public interface IEventService
    {
        
        Task<IEnumerable<EventDto>> GetAllAsync();

       
        Task<EventDto?> GetByIdAsync(Guid id);

        
        Task<EventDto> CreateAsync(CreateEventDto dto);

       
        Task UpdateAsync(Guid id, CreateEventDto dto);
    }
}

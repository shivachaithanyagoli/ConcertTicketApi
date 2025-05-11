using AutoMapper;
using ConcertTicketApi.Domain.Models;
using ConcertTicketApi.Api.Models;

namespace ConcertTicketApi.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Event, EventDto>();
            CreateMap<CreateEventDto, Event>();
            // Add mappings for TicketType and Reservation
        }
    }
}

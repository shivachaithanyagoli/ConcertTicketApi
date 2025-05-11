using AutoMapper;
using ConcertTicketApi.Domain.Models;
using ConcertTicketApi.Api.Models;

namespace ConcertTicketApi.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Event mappings
            CreateMap<Event, EventDto>();
            CreateMap<CreateEventDto, Event>();

            // TicketType → TicketTypeDto
            CreateMap<TicketType, TicketTypeDto>();

            // Reservation → ReservationDto
            CreateMap<Reservation, ReservationDto>();
        }
    }
}

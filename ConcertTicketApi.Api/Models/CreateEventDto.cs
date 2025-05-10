namespace ConcertTicketApi.Api.Models
{
    public class CreateEventDto
    {
        public string Name { get; set; } = "";
        public DateTime Date { get; set; }
        public string Venue { get; set; } = "";
        public string Description { get; set; } = "";
        public int Capacity { get; set; }
    }
}

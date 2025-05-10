using FluentValidation;
using ConcertTicketApi.Api.Models;

namespace ConcertTicketApi.Api.Validators
{
    public class CreateEventDtoValidator : AbstractValidator<CreateEventDto>
    {
        public CreateEventDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100);
            RuleFor(x => x.Date)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Date must be in the future");
            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be positive");
        }
    }
}

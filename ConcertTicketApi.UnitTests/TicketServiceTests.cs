// File: ConcertTicketApi.UnitTests/TicketServiceTests.cs

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ConcertTicketApi.Api.Services;
using ConcertTicketApi.Api.Models;
using ConcertTicketApi.Api.Mapping;
using ConcertTicketApi.Infrastructure;
using ConcertTicketApi.Domain.Models;

namespace ConcertTicketApi.UnitTests
{
    public class TicketServiceTests
    {
        private static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            return config.CreateMapper();
        }

        private static ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetAvailabilityAsync_ReturnsTicketTypesForEvent()
        {
            // Arrange
            var ctx    = CreateContext();
            var mapper = CreateMapper();
            var ev = new Event {
                Id               = Guid.NewGuid(),
                Name             = "E",
                Date             = DateTime.UtcNow,
                Venue            = "V",
                Description      = "D",
                Capacity         = 100
            };
            var tt1 = new TicketType {
                Id                = Guid.NewGuid(),
                EventId           = ev.Id,
                Event             = ev,
                Name              = "General",
                Price             = 50m,
                AvailableQuantity = 10
            };
            var tt2 = new TicketType {
                Id                = Guid.NewGuid(),
                EventId           = ev.Id,
                Event             = ev,
                Name              = "VIP",
                Price             = 100m,
                AvailableQuantity = 5
            };
            ctx.Events.Add(ev);
            ctx.TicketTypes.AddRange(tt1, tt2);
            await ctx.SaveChangesAsync();

            var svc = new TicketService(ctx, mapper);

            // Act
            var list = await svc.GetAvailabilityAsync(ev.Id);

            // Assert
            Assert.Equal(2, list.Count());
            Assert.Contains(list, t => t.Name == "General" && t.Price == 50m);
            Assert.Contains(list, t => t.Name == "VIP"     && t.Price == 100m);
        }

        [Fact]
        public async Task ReserveAsync_DecrementsQuantityAndReturnsReservation()
        {
            // Arrange
            var ctx    = CreateContext();
            var mapper = CreateMapper();
            var evId   = Guid.NewGuid();
            var tt = new TicketType {
                Id                = Guid.NewGuid(),
                EventId           = evId,
                Name              = "GA",
                Price             = 20m,
                AvailableQuantity = 2
            };
            ctx.TicketTypes.Add(tt);
            await ctx.SaveChangesAsync();

            var svc = new TicketService(ctx, mapper);

            // Act
            var resDto = await svc.ReserveAsync(evId, tt.Id, "Alice");

            // Assert
            Assert.Equal("Alice", resDto.CustomerName);
            Assert.Equal(tt.Id,     resDto.TicketTypeId);
            Assert.False(resDto.IsPurchased);
            var updated = await ctx.TicketTypes.FindAsync(tt.Id);
            Assert.NotNull(updated);
            Assert.Equal(1, updated!.AvailableQuantity);
        }

        [Fact]
        public async Task PurchaseAsync_MarksAsPurchased()
        {
            // Arrange
            var ctx    = CreateContext();
            var mapper = CreateMapper();
            var evId   = Guid.NewGuid();
            var tt = new TicketType {
                Id                = Guid.NewGuid(),
                EventId           = evId,
                Name              = "GA",
                Price             = 20m,
                AvailableQuantity = 1
            };
            var reservation = new Reservation {
                Id             = Guid.NewGuid(),
                TicketTypeId   = tt.Id,
                TicketType     = tt,
                CustomerName   = "Bob",
                ReservedAt     = DateTime.UtcNow,
                IsPurchased    = false
            };
            ctx.TicketTypes.Add(tt);
            ctx.Reservations.Add(reservation);
            await ctx.SaveChangesAsync();

            var svc = new TicketService(ctx, mapper);

            // Act
            var resDto = await svc.PurchaseAsync(evId, tt.Id, reservation.Id);

            // Assert
            Assert.True(resDto.IsPurchased);
            var loaded = await ctx.Reservations.FindAsync(reservation.Id);
            Assert.True(loaded!.IsPurchased);
        }

        [Fact]
        public async Task CancelAsync_RemovesReservationAndRestoresQuantity()
        {
            // Arrange
            var ctx    = CreateContext();
            var mapper = CreateMapper();
            var evId   = Guid.NewGuid();
            var tt = new TicketType {
                Id                = Guid.NewGuid(),
                EventId           = evId,
                Name              = "GA",
                Price             = 20m,
                AvailableQuantity = 0
            };
            var reservation = new Reservation {
                Id             = Guid.NewGuid(),
                TicketTypeId   = tt.Id,
                TicketType     = tt,
                CustomerName   = "Eve",
                ReservedAt     = DateTime.UtcNow,
                IsPurchased    = false
            };
            ctx.TicketTypes.Add(tt);
            ctx.Reservations.Add(reservation);
            await ctx.SaveChangesAsync();

            var svc = new TicketService(ctx, mapper);

            // Act
            await svc.CancelAsync(evId, tt.Id, reservation.Id);

            // Assert reservation removed
            var shouldBeNull = await ctx.Reservations.FindAsync(reservation.Id);
            Assert.Null(shouldBeNull);

            // Assert quantity restored
            var updated = await ctx.TicketTypes.FindAsync(tt.Id);
            Assert.NotNull(updated);
            Assert.Equal(1, updated!.AvailableQuantity);
        }
    }
}

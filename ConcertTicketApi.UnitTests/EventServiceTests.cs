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
    public class EventServiceTests
    {
        private static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            return config.CreateMapper();
        }

        private static ApplicationDbContext CreateContext()
        {
            // Build in-memory options
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_AddsEventToDatabase()
        {
            // Arrange
            var ctx    = CreateContext();
            var mapper = CreateMapper();
            var svc    = new EventService(ctx, mapper);

            var dto = new CreateEventDto
            {
                Name        = "Test Concert",
                Date        = DateTime.UtcNow.AddDays(5),
                Venue       = "Test Venue",
                Description = "Test Description",
                Capacity    = 123
            };

            // Act
            var result = await svc.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Name, result.Name);

            var stored = await ctx.Events.FindAsync(result.Id);
            Assert.NotNull(stored);
            Assert.Equal(dto.Venue, stored.Venue);
            Assert.Equal(dto.Capacity, stored.Capacity);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllSeededEvents()
        {
            // Arrange
            var ctx    = CreateContext();
            // seed two events directly
            ctx.Events.Add(new Event {
                Id          = Guid.NewGuid(),
                Name        = "E1",
                Date        = DateTime.UtcNow.AddDays(1),
                Venue       = "V1",
                Description = "D1",
                Capacity    = 10
            });
            ctx.Events.Add(new Event {
                Id          = Guid.NewGuid(),
                Name        = "E2",
                Date        = DateTime.UtcNow.AddDays(2),
                Venue       = "V2",
                Description = "D2",
                Capacity    = 20
            });
            await ctx.SaveChangesAsync();

            var mapper = CreateMapper();
            var svc    = new EventService(ctx, mapper);

            // Act
            var list = await svc.GetAllAsync();

            // Assert
            Assert.Equal(2, list.Count());
            Assert.Contains(list, e => e.Name == "E1");
            Assert.Contains(list, e => e.Name == "E2");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ConcertTicketApi.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ConcertTicketApi.IntegrationTests
{
    public class EventsApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public EventsApiTests(CustomWebApplicationFactory factory)
        {
            // Spins up the API in-memory
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllEvents_ReturnsOkAndEmptyList_WhenNoEvents()
        {
            // Act
            var response = await _client.GetAsync("/api/events");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var list = await response.Content.ReadFromJsonAsync<List<EventDto>>();
            Assert.NotNull(list);
            Assert.Empty(list);
        }

        [Fact]
        public async Task PostEvent_CreatesAndReturnsEvent()
        {
            // Arrange
            var dto = new CreateEventDto
            {
                Name        = "Integration Test Event",
                Date        = DateTime.UtcNow.AddDays(1),
                Venue       = "Test Venue",
                Description = "Desc",
                Capacity    = 100
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/events", dto);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var created = await response.Content.ReadFromJsonAsync<EventDto>();
            Assert.NotNull(created);
            Assert.Equal(dto.Name, created!.Name);

            // Cleanup: ensure we can retrieve it
            var getResponse = await _client.GetAsync($"/api/events/{created.Id}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        }
    }
}

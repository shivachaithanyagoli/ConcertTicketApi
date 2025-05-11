using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ConcertTicketApi.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ConcertTicketApi.IntegrationTests
{
    public class EventsApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public EventsApiTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();

            // 1) Log in to get JWT
            var loginResponse = _client.PostAsJsonAsync("/api/auth/login", new LoginDto {
                Username = "admin",
                Password = "p@ssw0rd"
            }).GetAwaiter().GetResult();
            loginResponse.EnsureSuccessStatusCode();

            // 2) Parse the JSON safely
            var dict = loginResponse.Content
                .ReadFromJsonAsync<Dictionary<string, string>>()
                .GetAwaiter().GetResult()
                ?? throw new InvalidOperationException("Login response was empty");

            if (!dict.TryGetValue("token", out var jwt) || string.IsNullOrEmpty(jwt))
                throw new InvalidOperationException("JWT token not found in login response");

            // 3) Attach to all requests
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwt);
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
            var dto = new CreateEventDto {
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

            // Cleanup: verify retrieval
            var getResponse = await _client.GetAsync($"/api/events/{created.Id}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        }
    }
}

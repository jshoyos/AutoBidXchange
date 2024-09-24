using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.IntegrationTests.Fixtures;
using AuctionService.IntegrationTests.Util;
using Contracts;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace AuctionService.IntegrationTests
{
    [Collection("SharedCollection")]
    public class AuctionBusTests : IAsyncLifetime
    {
        private readonly CustomWebAppFactory _factory;
        private readonly HttpClient _httpClient;
        private ITestHarness _harness;

        public AuctionBusTests(CustomWebAppFactory factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient();
            _harness = factory.Services.GetTestHarness();
        }

        [Fact]
        public async Task CreateAuction_WithValidObject_ShouldPublishAuctionCreated()
        {
            // arrange
            var auction = GetAuctionForCreate();
            _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("test"));

            // act
            var response = await _httpClient.PostAsJsonAsync("api/auctions", auction);

            // assert
            response.EnsureSuccessStatusCode();
            Assert.True(await _harness.Published.Any<AuctionCreated>());
        }

        public Task InitializeAsync() => Task.CompletedTask;

        public Task DisposeAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<AuctionDbContext>();
            DbHelper.ReInitForTests(db);
            return Task.CompletedTask;
        }

        private CreateAuctionDto GetAuctionForCreate()
        {
            return new CreateAuctionDto
            {
                Make = "Test",
                Model = "TestModel",
                ImageUrl = "TestUrl",
                Color = "TestColor",
                Mileage = 198_000,
                Year = 2014,
                ReservePrice = 10,
                AuctionEnd = DateTime.UtcNow.AddDays(5)
            };
        }
    }
}

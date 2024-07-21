using AuctionService.Data;
using AuctionService.IntegrationTests.Util;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using WebMotions.Fake.Authentication.JwtBearer;

namespace AuctionService.IntegrationTests.Fixtures
{
    public class CustomWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private PostgreSqlContainer _sqlContainer = new PostgreSqlBuilder().Build();

        public async Task InitializeAsync()
        {
            await _sqlContainer.StartAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureTestServices(services =>
            {
                services.RemoveDbContext();
                services.AddDbContext<AuctionDbContext>(options =>
                {
                    options.UseNpgsql(_sqlContainer.GetConnectionString());
                });

                services.AddMassTransitTestHarness();
                services.EnsureCreated();
                services.AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme)
                    .AddFakeJwtBearer(options =>
                    {
                        options.BearerValueType = FakeJwtBearerBearerValueType.Jwt;
                    });
            });
        }

        Task IAsyncLifetime.DisposeAsync() => _sqlContainer.DisposeAsync().AsTask();
    }
}

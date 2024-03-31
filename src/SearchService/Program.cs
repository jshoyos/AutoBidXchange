using System.Net;
using Contracts;
using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SearchService;
using SearchService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpClient<AuctionServiceHttpClient>().AddPolicyHandler(GetPolicy());
builder.Services.AddMassTransit(config =>
{
   config.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
   config.AddConsumersFromNamespaceContaining<AuctionUpdatedConsumer>();
   config.AddConsumersFromNamespaceContaining<AuctionDeletedConsumer>();

   config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

   config.UsingRabbitMq((context, cfg) =>
   {
      cfg.ReceiveEndpoint("search-auction-created", e =>
      {
         e.UseMessageRetry(r => r.Interval(5, 5));
         e.ConfigureConsumer<AuctionCreatedConsumer>(context);
      });

      cfg.ReceiveEndpoint("search-auction-update", e =>
      {
         e.UseMessageRetry(r => r.Interval(5, 5));
         e.ConfigureConsumer<AuctionUpdatedConsumer>(context);
      });

      cfg.ReceiveEndpoint("search-auction-deleted", e =>
      {
         e.UseMessageRetry(r => r.Interval(5, 5));
         e.ConfigureConsumer<AuctionDeletedConsumer>(context);
      });

      cfg.ConfigureEndpoints(context);
   });
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async () =>
{
   try
   {
      await DbInitializer.InitDb(app);
   }
   catch (Exception e)
   {
      Console.Error.WriteLine(e);
   }
});

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetPolicy()
   => HttpPolicyExtensions
   .HandleTransientHttpError()
   .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
   .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));

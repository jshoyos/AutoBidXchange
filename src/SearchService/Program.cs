using System.Net;
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

   config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

   config.UsingRabbitMq((context, cfg) =>
   {
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

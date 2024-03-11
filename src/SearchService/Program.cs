using SearchService;
using SearchService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpClient<AuctionServiceHttpClient>();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

try
{
   await DbInitializer.InitDb(app);
}
catch (Exception e)
{
   Console.Error.WriteLine(e);
}

app.Run();

using MassTransit;
using NotificationService.Consumers;
using NotificationService.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(config =>
{
    config.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("nt", false));

    config.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", host =>
        {
            host.Username(builder.Configuration.GetValue("RabbitMq:Usernmae", "guest"));
            host.Username(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
        });
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<NotificationHub>("/notifications");

app.Run();

using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Hubs;

namespace NotificationService.Consumers;

public class AuctionFinishedConsumer(IHubContext<NotificationHub> hubContext) : IConsumer<AuctionFinished>
{
    private readonly IHubContext<NotificationHub> _hubContext = hubContext;

    public Task Consume(ConsumeContext<AuctionFinished> context)
    {
        Console.WriteLine("---> auction finished event received");
        return _hubContext.Clients.All.SendAsync("AuctionFinished", context.Message);
    }
}

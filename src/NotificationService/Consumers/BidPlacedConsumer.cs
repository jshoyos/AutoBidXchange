using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Hubs;

namespace NotificationService.Consumers;

public class BidPlacedConsumer(IHubContext<NotificationHub> hubContext) : IConsumer<BidPlaced>
{
    private readonly IHubContext<NotificationHub> _hubContext = hubContext;
    public Task Consume(ConsumeContext<BidPlaced> context)
    {
        Console.WriteLine("---> bid placed event received");
        return _hubContext.Clients.All.SendAsync("BidPlaced", context.Message);
    }
}

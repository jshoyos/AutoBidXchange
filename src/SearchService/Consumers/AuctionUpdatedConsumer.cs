using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService;

public class AuctionUpdatedConsumer : IConsumer<AuctionUpdated>
{
   private readonly IMapper _autoMapper;

   public AuctionUpdatedConsumer(IMapper autoMapper)
   {
      _autoMapper = autoMapper;
   }

   public async Task Consume(ConsumeContext<AuctionUpdated> context)
   {
      Console.WriteLine($"---> Consuming Auction Updated: {context.Message.Id}");

      var item = _autoMapper.Map<Item>(context.Message);

      var result = await DB.Update<Item>()
         .MatchID(context.Message.Id)
         .ModifyOnly(x => new
         {
            x.Color,
            x.Make,
            x.Model,
            x.Year,
            x.Mileage
         }, item)
         .ExecuteAsync();

      if (!result.IsAcknowledged) throw new MessageException(typeof(AuctionUpdated), "Problem Updating mongodb");
   }
}

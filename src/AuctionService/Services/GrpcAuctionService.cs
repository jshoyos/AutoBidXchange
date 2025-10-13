using AuctionService.Data;
using Grpc.Core;

namespace AuctionService.Services;

public class GrpcAuctionService : GrpcAuction.GrpcAuctionBase
{
    private readonly AuctionDbContext _dbContext;

    public GrpcAuctionService(AuctionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<GrpcAuctionResponse> GetAuctions(GetAuctionRequest request, ServerCallContext context)
    {
        Console.WriteLine("===> Received gRPC request for auctions");

        var auction = await _dbContext.Auctions.FindAsync(Guid.Parse(request.Id)) ??
            throw new RpcException(new Status(StatusCode.NotFound, "Auction not found"));
            
        var response = new GrpcAuctionResponse
        {
            Auction = new GrpcAuctionModel
            {
                Id = auction.Id.ToString(),
                AuctionEnd = auction.AuctionEnd.ToString(),
                ReservePrice = auction.ReservePrice,
                Seller = auction.Seller
            }
        };

        return response;
    }
}
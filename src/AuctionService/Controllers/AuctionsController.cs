using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionService.Controllers
{
    [Route("api/auctions")]
   [ApiController]
   public class AuctionsController : ControllerBase
   {
        private readonly IAuctionRepository _repo;
        private readonly IMapper _mapper;
      private readonly IPublishEndpoint _publishEndpoint;

      public AuctionsController(IAuctionRepository repo, IMapper mapper, IPublishEndpoint publishEndpoint)
      {
        _repo = repo;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
      }

      [HttpGet]
      public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date)
      {
         return await _repo.GetAuctionsAsync(date);
      }

      [HttpGet("{id}")]
      public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid Id)
      {
         var auction = await _repo.GetAuctionByIdAsync(Id);

         if (auction is null) return NotFound();

            return auction;
      }

      [Authorize]
      [HttpPost]
      public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
      {
         var auction = _mapper.Map<Auction>(auctionDto);

         auction.Seller = User.Identity.Name;

         _repo.AddAuction(auction);

         var newAuction = _mapper.Map<AuctionDto>(auction);

         await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

         var result = await _repo.SaveChangesAsync();

         if (!result) return BadRequest("Could not save changes to the DB");

         return CreatedAtAction(nameof(GetAuctionById),
             new { auction.Id }, newAuction);
      }

      [Authorize]
      [HttpPut("{id}")]
      public async Task<ActionResult> UpdateAuction(Guid Id, UpdateAuctionDto updatedAuction)
      {
         var auction = await _repo.GetAuctionEntityByIdAsync(Id);

         if (auction is null) return NotFound(Id);

         if (auction.Seller != User.Identity.Name) return Forbid();

         auction.Item.Make = updatedAuction.Make ?? auction.Item.Make;
         auction.Item.Model = updatedAuction.Model ?? auction.Item.Model;
         auction.Item.Color = updatedAuction.Color ?? auction.Item.Color;
         auction.Item.Mileage = updatedAuction.Mileage ?? auction.Item.Mileage;
         auction.Item.Year = updatedAuction.Year ?? auction.Item.Year;

         await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

         var result = await _repo.SaveChangesAsync();

         if (result) return Ok();

         return BadRequest("Unable to save changes");
      }


      [Authorize]
      [HttpDelete("{id}")]
      public async Task<ActionResult> DeleteAuction(Guid Id)
      {
        var auctionToDelete = await _repo.GetAuctionEntityByIdAsync(Id);

         if (auctionToDelete is null) return NotFound(Id);

         if (auctionToDelete.Seller != User.Identity.Name) return Forbid();

         _repo.RemoveAuction(auctionToDelete);

         await _publishEndpoint.Publish<AuctionDeleted>(new { Id = auctionToDelete.Id.ToString() });

         var result = await _repo.SaveChangesAsync();

         if (result) return Ok();

         return BadRequest("Unable to delete auction");
      }
   }
}

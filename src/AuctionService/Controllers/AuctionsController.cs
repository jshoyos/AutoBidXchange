﻿using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [Route("api/auctions")]
    [ApiController]
    public class AuctionsController : ControllerBase
    {
        private readonly AuctionDbContext _dbContext;
        private readonly IMapper _mapper;

        public AuctionsController(AuctionDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllActions()
        {
            var auctions = await _dbContext.Auctions
                .Include(x => x.Item)
                .OrderBy(x => x.Item.Make)
                .ToListAsync();

            return _mapper.Map<List<AuctionDto>>(auctions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid Id)
        {
            var auction = await _dbContext.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == Id);

            if (auction is null) return NotFound();

            return _mapper.Map<AuctionDto>(auction);
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
        {
            var auction = _mapper.Map<Auction>(auctionDto);
            // TODO: add current user as seller
            auction.Seller = "test";

            _dbContext.Auctions.Add(auction);

            var result = await _dbContext.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Could not save changes to the DB");

            return CreatedAtAction(nameof(GetAuctionById), 
                new {auction.Id}, _mapper.Map<AuctionDto>(auction));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid Id, UpdateAuctionDto updatedAuction)
        {
            var auction = await _dbContext.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == Id);

            if (auction is null) return NotFound(Id);

            // TODO: check seller == username

            auction.Item.Make = updatedAuction.Make ?? auction.Item.Make;
            auction.Item.Model = updatedAuction.Model ?? auction.Item.Model;
            auction.Item.Color = updatedAuction.Color ?? auction.Item.Color;
            auction.Item.Mileage = updatedAuction.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = updatedAuction.Year ?? auction.Item.Year;

            var result = await _dbContext.SaveChangesAsync() > 0;

            if (result) return Ok();

            return BadRequest("Unable to save changes");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid Id)
        {
            var auctionToDelete = await _dbContext.Auctions
                .FirstOrDefaultAsync(x => x.Id == Id);

            if (auctionToDelete is null) return NotFound(Id);

            // TODO: chech is seller == username

            _dbContext.Auctions.Remove(auctionToDelete);

            var result = await _dbContext.SaveChangesAsync() > 0;

            if (result) return Ok();

            return BadRequest("Unable to delete auction");
        }
    }
}
using AuctionService.Controllers;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.RequestHelpers;
using AuctionService.UnitTests.Utils;
using AutoFixture;
using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace AuctionService.UnitTests;

public class AuctionControllerTests
{
    private readonly Mock<IAuctionRepository> _repo;
    private readonly Mock<IPublishEndpoint> _endpoint;
    private readonly IMapper _mapper;
    private readonly AuctionsController _controller;
    private readonly Fixture _fixture;

    public AuctionControllerTests()
    {
        _fixture = new Fixture();
        _repo = new Mock<IAuctionRepository>();
        _endpoint = new Mock<IPublishEndpoint>();

        var mockMapper = new MapperConfiguration(mc =>
        {
            mc.AddMaps(typeof(MappingProfiles).Assembly);
        }).CreateMapper().ConfigurationProvider;
        _mapper = new Mapper(mockMapper);

        _controller = new AuctionsController(_repo.Object, _mapper, _endpoint.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = Helpers.GetClaimsPrincipal() }
            }
        };
    }

    [Fact]
    public async Task GetAuctions_WithNoParams_Returns10Auctions()
    {
        // Arrange
        var auctions = _fixture.CreateMany<AuctionDto>(10).ToList();
        _repo.Setup(x => x.GetAuctionsAsync(null)).ReturnsAsync(auctions);

        // Act
        var result = await _controller.GetAllAuctions(null);

        // Assert
        Assert.Equal(10, result?.Value?.Count);
        Assert.IsType<ActionResult<List<AuctionDto>>>(result);
    }

    [Fact]
    public async Task GetAuctionById_WithExistingId_ReturnsAuction()
    {
        // Arrange
        var auction = _fixture.Create<AuctionDto>();
        _repo.Setup(x => x.GetAuctionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);

        // Act
        var result = await _controller.GetAuctionById(auction.Id);

        // Assert
        Assert.Equal(auction, result?.Value);
        Assert.IsType<ActionResult<AuctionDto>>(result);
    }

    [Fact]
    public async Task GetAuctionById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _repo.Setup(x => x.GetAuctionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(value:null) ;

        // Act
        var result = await _controller.GetAuctionById(Guid.NewGuid());
        var notFoundResult = result.Result as NotFoundResult;

        // Assert
        Assert.NotNull(notFoundResult);
        Assert.IsType<NotFoundResult>(notFoundResult);
    }

    [Fact]
    public async Task CreateAuction_WithValidParam_ReturnsCreatedAtAction()
    {
        // arrange
        var auctionDto = _fixture.Create<CreateAuctionDto>();
        _repo.Setup(x => x.AddAuction(It.IsAny<Auction>()));
        _repo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);

        // act
        var result = await _controller.CreateAuction(auctionDto);
        var createdResult = result.Result as CreatedAtActionResult;

        // assert
        Assert.NotNull(createdResult);
        Assert.Equal("GetAuctionById", createdResult.ActionName);
        Assert.IsType<AuctionDto>(createdResult.Value);
    }

    [Fact]
    public async Task CreateAuction_WithInvalidCreationDto_ReturnsBadRequest()
    {
        // arrange
        var auctionDto = _fixture.Create<CreateAuctionDto>();
        _repo.Setup(x => x.AddAuction(It.IsAny<Auction>()));
        _repo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);

        // act
        var result = await _controller.CreateAuction(auctionDto);
        var createdResult = result.Result as BadRequestObjectResult;

        // assert
        Assert.NotNull(createdResult);
    }

    [Fact]
    public async Task UpdateAuction_WithValidParameters_ReturnsOk()
    {
        // arrange
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
        auction.Seller = "test";
        var auctionUpdateDto = _fixture.Create<UpdateAuctionDto>();
        _repo.Setup(x => x.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);
        _repo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);

        // act
        var result = await _controller.UpdateAuction(auction.Id, auctionUpdateDto);

        // assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task UpdateAuction_WithInvalidId_ReturnsNotFound()
    {
        // arrange
        _repo.Setup(x => x.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(value: null);
        var updateAuctionDto = _fixture.Create<UpdateAuctionDto>();

        // act
        var result = await _controller.UpdateAuction(Guid.NewGuid(), updateAuctionDto);
        var notFoundResult = result as NotFoundObjectResult;

        // assert
        Assert.NotNull(notFoundResult);
        Assert.IsType<NotFoundObjectResult>(notFoundResult);
    }

    [Fact]
    public async Task UpdateAuction_WithInvalidUser_ReturnsForbid()
    {
        // arrange
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Seller = "no-test";
        var updateAuctionDto = _fixture.Create<UpdateAuctionDto>();

        _repo.Setup(x => x.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);

        // act
        var result = await _controller.UpdateAuction(auction.Id, updateAuctionDto);

        // assert
        Assert.IsType<ForbidResult>(result);

    }

    [Fact]
    public async Task UpdateAuction_FailToSave_ReturnsBadRequest()
    {
        // arrange
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
        auction.Seller = "test";
        var updateAuctionDto = _fixture.Create<UpdateAuctionDto>();

        _repo.Setup(x => x.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);
        _repo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);

        // act
        var result = await _controller.UpdateAuction(auction.Id, updateAuctionDto);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteAuction_WithValidId_ReturnsOk()
    {
        // arrange
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Seller = "test";

        _repo.Setup(x => x.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);
        _repo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);

        // act
        var result = await _controller.DeleteAuction(auction.Id);

        // assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteAuction_WithInvalidId_ReturnsNotFound()
    {
        // arrange
        _repo.Setup(x => x.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(value: null);

        // act
        var result = await _controller.DeleteAuction(Guid.NewGuid());

        // assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeletecAution_FailToSave_ReturnsBadRequest()
    {
        // arrange
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Seller = "test";

        _repo.Setup(x => x.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);
        _repo.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);

        // act
        var result = await _controller.DeleteAuction(auction.Id);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteAuction_WithInvavlidUser_ReturnsForbid()
    {
        // arrange
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Seller = "no-test";

        _repo.Setup(x => x.GetAuctionEntityByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);

        // act
        var result = await _controller.DeleteAuction(auction.Id);

        // assert
        Assert.IsType<ForbidResult>(result);
    }
}
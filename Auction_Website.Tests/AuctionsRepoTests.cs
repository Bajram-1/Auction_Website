using Auction_Website.DAL;
using Auction_Website.DAL.Entities;
using Auction_Website.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Auction_Website.Tests
{
    public class AuctionRepositoryTests
    {
        private ApplicationDbContext GetDbContextWithData()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            var users = new List<ApplicationUser>
    {
        new ApplicationUser { Id = "User123", FirstName = "Alice", LastName = "Doe", Email = "alice@example.com" },
        new ApplicationUser { Id = "User456", FirstName = "Bob", LastName = "Smith", Email = "bob@example.com" },
        new ApplicationUser { Id = "User789", FirstName = "Charlie", LastName = "Brown", Email = "charlie@example.com" }
    };

            context.Users.AddRange(users);
            context.SaveChanges();

            context.Auctions.AddRange(new List<Auction>
    {
        new Auction {
            AuctionId = 1,
            Title = "Test Auction 1",
            Description = "Description 1",
            EndTime = DateTime.UtcNow.AddDays(2),
            IsClosed = false,
            CreatedByUserId = "User123"
        },
        new Auction {
            AuctionId = 2,
            Title = "Test Auction 2",
            Description = "Description 2",
            EndTime = DateTime.UtcNow.AddDays(-1),
            IsClosed = false,
            CreatedByUserId = "User456"
        },
        new Auction {
            AuctionId = 3,
            Title = "Test Auction 3",
            Description = "Description 3",
            EndTime = DateTime.UtcNow.AddDays(5),
            IsClosed = false,
            CreatedByUserId = "User789"
        }
    });

            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task GetActiveAuctionsAsync_ShouldReturnOnlyActiveAuctions()
        {
            using var context = GetDbContextWithData();
            var repository = new AuctionRepository(context);

            var result = await repository.GetActiveAuctionsAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAuctionByIdAsync_ShouldReturnCorrectAuction()
        {
            using var context = GetDbContextWithData();
            var repository = new AuctionRepository(context);

            var auction = await repository.GetAuctionByIdAsync(1);

            Assert.NotNull(auction);
            Assert.Equal("Test Auction 1", auction.Title);
        }

        [Fact]
        public async Task GetAuctionByIdAsync_ShouldReturnNull_WhenAuctionDoesNotExist()
        {
            using var context = GetDbContextWithData();
            var repository = new AuctionRepository(context);

            var auction = await repository.GetAuctionByIdAsync(99);

            Assert.Null(auction);
        }
    }
}
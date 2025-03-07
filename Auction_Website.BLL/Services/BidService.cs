using Auction_Website.BLL.DTO.Requests;
using Auction_Website.BLL.IServices;
using Auction_Website.DAL;
using Auction_Website.DAL.Entities;
using Auction_Website.DAL.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auction_Website.BLL.Services
{
    public class BidService : IBidService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILoggerService _loggerService;

        public BidService(IUnitOfWork unitOfWork, ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILoggerService loggerService)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _userManager = userManager;
            _loggerService = loggerService;
        }

        public async Task<bool> PlaceBidAsync(BidAddRequestModel model, string userId)
        {
            try
            {
                var auction = await _context.Auctions
                    .Include(a => a.Bids)
                    .FirstOrDefaultAsync(a => a.AuctionId == model.AuctionId);

                if (auction == null || auction.IsClosed || auction.EndTime < DateTime.UtcNow)
                {
                    return false;
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null || user.WalletBalance < model.Amount)
                {
                    return false;
                }

                var highestBid = auction.Bids.OrderByDescending(b => b.Amount).FirstOrDefault();

                if (highestBid != null && model.Amount <= highestBid.Amount)
                {
                    return false;
                }

                if (highestBid != null)
                {
                    var previousBidder = await _context.Users.FindAsync(highestBid.UserId);
                    if (previousBidder != null)
                    {
                        previousBidder.WalletBalance += highestBid.Amount;
                    }
                }

                user.WalletBalance -= model.Amount;

                var bid = new DAL.Entities.Bid
                {
                    AuctionId = model.AuctionId,
                    UserId = userId,
                    Amount = model.Amount,
                    TimePlaced = DateTime.UtcNow
                };

                _context.Bids.Add(bid);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _loggerService.LogError($"Error placing bid: {ex.Message}");
                return false;
            }
        }
    }
}
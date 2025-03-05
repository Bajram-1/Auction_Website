using Auction_Website.BLL.DTO;
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

        public BidService(IUnitOfWork unitOfWork, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> PlaceBidAsync(BidAddRequestModel model, string userId)
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

            var highestBid = auction.Bids.Any() ? auction.Bids.Max(b => b.Amount) : auction.StartingPrice;
            if (model.Amount <= highestBid)
            {
                return false; 
            }

            var bid = new DAL.Entities.Bid
            {
                AuctionId = model.AuctionId,
                UserId = userId,
                Amount = model.Amount
            };

            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
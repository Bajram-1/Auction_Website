using Auction_Website.BLL.DTO.Requests;
using Auction_Website.BLL.IServices;
using Auction_Website.DAL;
using Auction_Website.DAL.Entities;
using Auction_Website.DAL.IRepositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Auction_Website.UI.Hubs;

namespace Auction_Website.BLL.Services
{
    public class AuctionService : IAuctionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoggerService _logger;
        private readonly IHubContext<WalletHub> _walletHub;

        public AuctionService(ApplicationDbContext context, ILoggerService logger, IUnitOfWork unitOfWork, IMemoryCache cache, IHubContext<WalletHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _logger = logger;
            _walletHub = hubContext;
        }

        public async Task<IEnumerable<AuctionAddEditRequestModel>> GetActiveAuctionsAsync()
        {
            var auctions = await _unitOfWork.AuctionRepository.GetActiveAuctionsAsync();

            return auctions
                .Where(a => a.EndTime > DateTime.UtcNow && !a.IsClosed)
                .OrderBy(a => a.EndTime)
                .Select(a => new AuctionAddEditRequestModel
                {
                    AuctionId = a.AuctionId,
                    Title = a.Title,
                    StartingPrice = a.StartingPrice,
                    EndTime = a.EndTime,
                    IsClosed = a.IsClosed,
                    Description = a.Description,
                    SellerName = a.CreatedByUser != null ? $"{a.CreatedByUser.FirstName} {a.CreatedByUser.LastName}" : "Unknown",
                    CreatedByUserId = a.CreatedByUser?.Id ?? "Unknown",
                    CurrentHighestBid = a.Bids.Any() ? a.Bids.Max(b => b.Amount) : a.StartingPrice
                })
                .ToList();
        }

        public async Task<DAL.Entities.Auction> GetAuctionByIdAsync(int auctionId)
        {
            return await _unitOfWork.AuctionRepository.GetAuctionByIdAsync(auctionId);
        }

        public async Task CreateAuctionAsync(DAL.Entities.Auction auction)
        {
            await _unitOfWork.AuctionRepository.AddAsync(auction);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> PlaceBidAsync(int auctionId, string userId, decimal amount)
        {
            var auction = await _unitOfWork.AuctionRepository.GetAuctionByIdAsync(auctionId);
            if (auction == null || auction.IsClosed || auction.EndTime < System.DateTime.UtcNow)
                return false;

            var highestBid = await _unitOfWork.BidRepository.GetHighestBidAsync(auctionId);
            if (highestBid != null && amount <= highestBid.Amount)
                return false;

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null || user.WalletBalance < amount)
                return false;

            var bid = new DAL.Entities.Bid
            {
                AuctionId = auctionId,
                UserId = userId,
                Amount = amount
            };

            user.WalletBalance -= amount;
            await _unitOfWork.BidRepository.AddAsync(bid);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private DateTime GetAlbaniaTime(DateTime utcTime)
        {
            TimeZoneInfo albaniaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, albaniaTimeZone);
        }

        public async Task<bool> CloseAuction(int auctionId)
        {
            var auction = await _context.Auctions
                .Include(a => a.Bids)
                .FirstOrDefaultAsync(a => a.AuctionId == auctionId);

            if (auction == null)
            {
                _logger.LogWarning($"Auction ID {auctionId} not found.");
                return false;
            }

            DateTime albaniaTimeNow = GetAlbaniaTime(DateTime.UtcNow);

            if (albaniaTimeNow >= auction.EndTime)
            {
                auction.IsClosed = true;
                await _unitOfWork.SaveChangesAsync();
            }

            var highestBid = auction.Bids.OrderByDescending(b => b.Amount).FirstOrDefault();
            var seller = await _context.Users.FirstOrDefaultAsync(u => u.Id == auction.CreatedByUserId);

            if (highestBid != null)
            {
                var winner = await _context.Users.FirstOrDefaultAsync(u => u.Id == highestBid.UserId);

                if (winner == null || seller == null)
                {
                    _logger.LogError($"Error retrieving winner or seller for auction ID {auctionId}");
                    return false;
                }

                if (winner != null && seller != null && winner.WalletBalance >= highestBid.Amount)
                {
                    _logger.LogInfo($"Transferring ${highestBid.Amount} from {winner.UserName} to {seller.UserName} for auction ID {auctionId}.");

                    winner.WalletBalance -= highestBid.Amount;
                    seller.WalletBalance += highestBid.Amount;

                    var transfer = new Transfer
                    {
                        Amount = highestBid.Amount,
                        FromUserId = winner.Id,
                        ToUserId = seller.Id,
                        Reason = $"Payment for auction {auction.Title}"
                    };

                    _context.Transfers.Add(transfer);

                    auction.IsClosed = true;
                    await _unitOfWork.SaveChangesAsync();

                    await _walletHub.Clients.User(seller.Id).SendAsync("ReceiveWalletUpdate", seller.WalletBalance);
                    await _walletHub.Clients.User(winner.Id).SendAsync("ReceiveWalletUpdate", winner.WalletBalance);
                    await _walletHub.Clients.All.SendAsync("AuctionClosed", auction.AuctionId);

                    _logger.LogInfo($"Auction ID {auctionId} closed successfully.");
                    return true;
                }
                else
                {
                    _logger.LogWarning($"Winner {winner.UserName} does not have enough balance for auction ID {auctionId}.");
                    return false;
                }
            }
            else
            {
                _logger.LogInfo($"No bids found for auction ID {auctionId}. Closing auction.");
                auction.IsClosed = true;
                await _unitOfWork.SaveChangesAsync();

                await _walletHub.Clients.All.SendAsync("AuctionClosed", auction.AuctionId);

                return true;
            }
        }

        public async Task CloseExpiredAuctions()
        {
            DateTime albaniaTimeNow = GetAlbaniaTime(DateTime.UtcNow);

            var expiredAuctions = await _context.Auctions
                .Where(a => a.EndTime <= albaniaTimeNow && !a.IsClosed)
                .ToListAsync();

            if (!expiredAuctions.Any())
            {
                _logger.LogInfo("No expired auctions found.");
                return;
            }

            _logger.LogInfo($"Found {expiredAuctions.Count} expired auctions.");

            foreach (var auction in expiredAuctions)
            {
                _logger.LogInfo($"Closing auction ID: {auction.AuctionId}, Title: {auction.Title}");
                await CloseAuction(auction.AuctionId);
            }

            _logger.LogInfo("Expired auctions processed.");
        }

        public async Task<bool> DeleteAuctionAsync(int auctionId, string userId)
        {
            var auction = await _unitOfWork.AuctionRepository.GetAuctionByIdAsync(auctionId);

            if (auction == null || auction.CreatedByUserId != userId)
                return false;

            _unitOfWork.AuctionRepository.Delete(auction);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
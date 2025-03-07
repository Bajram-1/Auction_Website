using Auction_Website.BLL.DTO;
using Auction_Website.BLL.DTO.Requests;
using Auction_Website.BLL.DTO.ViewModels;
using Auction_Website.DAL.Entities;

namespace Auction_Website.BLL.IServices
{
    public interface IAuctionService
    {
        Task<IEnumerable<AuctionAddEditRequestModel>> GetActiveAuctionsAsync();
        Task<DAL.Entities.Auction> GetAuctionByIdAsync(int auctionId);
        Task CreateAuctionAsync(DAL.Entities.Auction auction);
        Task<bool> PlaceBidAsync(int auctionId, string userId, decimal bidAmount);
        Task<bool> CloseAuctionAsync(int auctionId);
        Task CloseExpiredAuctions();
        Task<bool> DeleteAuctionAsync(int auctionId, string userId);
    }
}
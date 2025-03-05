using Auction_Website.BLL.DTO;
using Auction_Website.BLL.DTO.Requests;
using Auction_Website.DAL.Entities;

namespace Auction_Website.BLL.IServices
{
    public interface IBidService
    {
        Task<bool> PlaceBidAsync(BidAddRequestModel model, string userId);
    }
}
using Auction_Website.DAL.Entities;

namespace Auction_Website.DAL.IRepositories
{
    public interface IAuctionRepository : IRepository<Auction>
    {
        Task<IEnumerable<Auction>> GetActiveAuctionsAsync();
        Task<Auction> GetAuctionByIdAsync(int auctionId);
    }
}
using Auction_Website.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auction_Website.DAL.IRepositories
{
    public interface IBidRepository : IRepository<Bid>
    {
        Task<IEnumerable<Bid>> GetBidsByAuctionIdAsync(int auctionId);
        Task<Bid> GetHighestBidAsync(int auctionId);
    }
}
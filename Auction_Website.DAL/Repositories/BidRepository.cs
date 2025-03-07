using Auction_Website.DAL.Entities;
using Auction_Website.DAL.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auction_Website.DAL.Repositories
{
    public class BidRepository : Repository<Bid>, IBidRepository
    {
        private readonly ApplicationDbContext _context;

        public BidRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Bid> GetHighestBidAsync(int auctionId)
        {
            return await _context.Bids
                .Where(b => b.AuctionId == auctionId)
                .OrderByDescending(b => b.Amount)
                .FirstOrDefaultAsync();
        }
    }
}
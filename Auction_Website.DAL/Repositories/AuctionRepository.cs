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
    public class AuctionRepository : Repository<Auction>, IAuctionRepository
    {
        public AuctionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Auction>> GetActiveAuctionsAsync()
        {
            return await _context.Auctions
                .Where(a => a.EndTime > System.DateTime.UtcNow && !a.IsClosed)
                .OrderBy(a => a.EndTime)
                .ToListAsync();
        }

        public async Task<Auction> GetAuctionByIdAsync(int auctionId)
        {
            return await _context.Auctions.FirstOrDefaultAsync(a => a.AuctionId == auctionId);
        }

        public async Task<IEnumerable<Auction>> GetAuctionsByUserIdAsync(string userId)
        {
            return await _context.Auctions
                .Where(a => a.CreatedByUserId == userId)
                .ToListAsync();
        }
    }
}
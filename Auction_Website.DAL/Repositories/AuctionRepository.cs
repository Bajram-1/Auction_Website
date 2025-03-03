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
        private readonly ApplicationDbContext _context;

        public AuctionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Auction>> GetActiveAuctionsAsync()
        {
            return await _context.Auctions
                .Where(a => a.EndTime > System.DateTime.UtcNow && !a.IsClosed)
                .OrderBy(a => a.EndTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Auction>> GetAuctionsByUserIdAsync(int userId)
        {
            return await _context.Auctions
                .Where(a => a.CreatedByUserId == userId)
                .ToListAsync();
        }
    }
}
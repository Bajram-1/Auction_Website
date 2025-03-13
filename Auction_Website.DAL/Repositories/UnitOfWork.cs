using Auction_Website.DAL.IRepositories;

namespace Auction_Website.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public IAuctionRepository AuctionRepository { get; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            AuctionRepository = new AuctionRepository(_db);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
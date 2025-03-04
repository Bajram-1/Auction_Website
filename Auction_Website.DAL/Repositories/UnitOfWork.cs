using Auction_Website.DAL.IRepositories;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auction_Website.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public IUserRepository UserRepository { get; }
        public IAuctionRepository AuctionRepository { get; }
        public IBidRepository BidRepository { get; }
        public ITransferRepository TransferRepository { get; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            UserRepository = new UserRepository(_db);
            AuctionRepository = new AuctionRepository(_db);
            BidRepository = new BidRepository(_db);
            TransferRepository = new TransferRepository(_db);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task<T> ExecuteTransactionAsync<T>(Func<Task<T>> func)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var result = await func();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _db.Database.BeginTransactionAsync();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
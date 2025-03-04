using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auction_Website.DAL.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IAuctionRepository AuctionRepository { get; }
        IBidRepository BidRepository { get; }
        ITransferRepository TransferRepository { get; }

        Task SaveChangesAsync();
        Task<T> ExecuteTransactionAsync<T>(Func<Task<T>> func);
        Task<IDbContextTransaction> BeginTransactionAsync();
        void Dispose();
    }
}
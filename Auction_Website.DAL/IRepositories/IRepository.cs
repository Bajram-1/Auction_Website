using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Auction_Website.DAL.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(string id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
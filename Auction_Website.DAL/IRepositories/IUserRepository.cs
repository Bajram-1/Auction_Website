using Auction_Website.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auction_Website.DAL.IRepositories
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        Task<ApplicationUser> GetByEmailAsync(string email);
    }
}

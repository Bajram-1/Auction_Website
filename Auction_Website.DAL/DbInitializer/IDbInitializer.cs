using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auction_Website.DAL.DbInitializer
{
    public interface IDbInitializer
    {
        Task InitializeAsync();
    }
}
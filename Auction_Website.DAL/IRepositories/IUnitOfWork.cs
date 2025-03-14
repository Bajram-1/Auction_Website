﻿using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auction_Website.DAL.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IAuctionRepository AuctionRepository { get; }

        Task SaveChangesAsync();
    }
}
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Auction_Website.DAL.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [DataType(DataType.Currency)]
        public decimal Wallet { get; set; } = 1000.00m;
        public virtual ICollection<Auction> AuctionsCreated { get; set; } = new List<Auction>();
        public virtual ICollection<Bid> Bids { get; set; } = new List<Bid>();
        public virtual ICollection<Transfer> TransfersSent { get; set; } = new List<Transfer>();
        public virtual ICollection<Transfer> TransfersReceived { get; set; } = new List<Transfer>();
    }
}
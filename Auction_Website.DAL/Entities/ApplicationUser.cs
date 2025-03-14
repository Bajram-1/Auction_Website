﻿using Microsoft.AspNetCore.Identity;

namespace Auction_Website.DAL.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal WalletBalance { get; set; } = 1000.00m;
        public bool IsActive { get; set; } = true;
        public virtual ICollection<Auction> AuctionsCreated { get; set; } = new List<Auction>();
        public virtual ICollection<Bid> Bids { get; set; } = new List<Bid>();
        public virtual ICollection<Transfer> TransfersSent { get; set; } = new List<Transfer>();
        public virtual ICollection<Transfer> TransfersReceived { get; set; } = new List<Transfer>();
    }
}
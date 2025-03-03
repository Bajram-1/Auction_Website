using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Auction_Website.DAL.Entities
{
    public class ApplicationUser
    {
        public int UserId { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Username { get; set; }
        [Required]
        [MinLength(8)]
        public string PasswordHash { get; set; }
        [Required]
        public string Email { get; set; }
        [DataType(DataType.Currency)]
        public decimal Wallet { get; set; } = 1000.00m;
        public ICollection<Auction> AuctionsCreated { get; set; }
        public ICollection<Bid> Bids { get; set; }
        public ICollection<Transfer> TransfersSent { get; set; }
        public ICollection<Transfer> TransfersReceived { get; set; }
    }
}
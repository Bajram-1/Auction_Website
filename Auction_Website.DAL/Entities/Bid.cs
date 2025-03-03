using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auction_Website.DAL.Entities
{
    public class Bid
    {
        public int BidId { get; set; }
        [Required]
        public int AuctionId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        [Required]
        public DateTime BidTime { get; set; } = DateTime.UtcNow;

        public Auction Auction { get; set; }
        public ApplicationUser User { get; set; }
    }
}
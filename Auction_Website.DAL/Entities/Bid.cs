using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auction_Website.DAL.Entities
{
    public class Bid
    {
        [Key]
        public int BidId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int AuctionId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public DateTime BidTime { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("AuctionId")]
        public virtual Auction Auction { get; set; }
    }
}
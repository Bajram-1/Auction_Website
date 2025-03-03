using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Auction_Website.DAL.Entities
{
    public class Auction
    {
        public int AuctionId { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [DataType(DataType.Currency)]
        public decimal StartingPrice { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        public bool IsClosed { get; set; } = false;
        public int CreatedByUserId { get; set; }
        public ApplicationUser CreatedByUser { get; set; }
        public ICollection<Bid> Bids { get; set; }
    }
}
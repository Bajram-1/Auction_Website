using System.ComponentModel.DataAnnotations;

namespace Auction_Website.BLL.DTO.Requests
{
    public class BidAddRequestModel
    {
        [Required]
        public int AuctionId { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Bid amount must be greater than zero.")]
        public decimal Amount { get; set; }
    }
}
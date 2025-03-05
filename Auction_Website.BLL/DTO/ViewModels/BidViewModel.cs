namespace Auction_Website.BLL.DTO.ViewModels
{
    public class BidViewModel
    {
        public string BidderName { get; set; }
        public decimal Amount { get; set; }
        public DateTime TimePlaced { get; set; }
    }
}
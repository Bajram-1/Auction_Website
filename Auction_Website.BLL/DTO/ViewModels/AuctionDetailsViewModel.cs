using TimeZoneConverter;

namespace Auction_Website.BLL.DTO.ViewModels
{
    public class AuctionDetailsViewModel
    {
        public int AuctionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SellerName { get; set; }
        public decimal StartingPrice { get; set; }
        public DateTime EndTime { get; set; }
        public string CreatedByUserId { get; set; }
        public bool IsClosed { get; set; }
        public decimal CurrentHighestBid { get; set; }
        public TimeSpan TimeRemaining
        {
            get
            {
                var albaniaTimeZone = TZConvert.GetTimeZoneInfo("Central European Standard Time");
                var localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, albaniaTimeZone);
                return EndTime - localNow;
            }
        }
        public List<BidViewModel> Bids { get; set; } = new List<BidViewModel>();
    }
}
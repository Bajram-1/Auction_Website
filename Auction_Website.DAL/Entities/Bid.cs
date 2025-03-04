namespace Auction_Website.DAL.Entities
{
    public class Bid
    {
        public int BidId { get; set; }
        public string UserId { get; set; }
        public int AuctionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime BidTime { get; set; } = DateTime.UtcNow;
        public virtual ApplicationUser User { get; set; }
        public virtual Auction Auction { get; set; }
    }
}
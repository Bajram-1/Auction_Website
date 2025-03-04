namespace Auction_Website.DAL.Entities
{
    public class Transfer
    {
        public int TransferId { get; set; }
        public decimal Amount { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public DateTime TransferDate { get; set; } = DateTime.UtcNow;
        public string Reason { get; set; }
        public virtual ApplicationUser FromUser { get; set; }
        public virtual ApplicationUser ToUser { get; set; }
    }
}
namespace Auction_Website.BLL.IServices
{
    public interface IAuctionExpirationService
    {
        Task CheckAndExpireAuctionsAsync();
    }
}
using Microsoft.AspNetCore.SignalR;

namespace Auction_Website.UI.Hubs
{
    public class WalletHub : Hub
    {
        public async Task SendWalletUpdate(string userId, decimal newBalance)
        {
            await Clients.User(userId).SendAsync("ReceiveWalletUpdate", newBalance);
        }

        public async Task AuctionClosed(int auctionId)
        {
            await Clients.All.SendAsync("AuctionClosed", auctionId);
        }
    }
}
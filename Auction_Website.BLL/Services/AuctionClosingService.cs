using Auction_Website.BLL.IServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Auction_Website.BLL.Services
{
    public class AuctionClosingService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AuctionClosingService> _logger;

        public AuctionClosingService(IServiceScopeFactory scopeFactory, ILogger<AuctionClosingService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AuctionClosingService started...");

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var auctionService = scope.ServiceProvider.GetRequiredService<IAuctionService>();

                    try
                    {
                        _logger.LogInformation("Checking for expired auctions...");
                        await auctionService.CloseExpiredAuctions();
                        _logger.LogInformation("Expired auctions processed.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error while closing auctions: {ex.Message}");
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }
    }
}
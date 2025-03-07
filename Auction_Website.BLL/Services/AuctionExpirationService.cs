using Auction_Website.BLL.IServices;
using Auction_Website.DAL.IRepositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace Auction_Website.BLL.Services
{
    public class AuctionExpirationService : BackgroundService, IAuctionExpirationService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AuctionExpirationService> _logger;

        public AuctionExpirationService(IServiceScopeFactory scopeFactory, ILogger<AuctionExpirationService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task CheckAndExpireAuctionsAsync()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var activeAuctions = await unitOfWork.AuctionRepository.GetActiveAuctionsAsync();
                var expiredAuctions = activeAuctions.Where(a => a.EndTime <= DateTime.UtcNow).ToList();

                foreach (var auction in expiredAuctions)
                {
                    auction.IsClosed = true;
                    unitOfWork.AuctionRepository.Update(auction);
                }

                if (expiredAuctions.Any())
                {
                    await unitOfWork.SaveChangesAsync();
                    _logger.LogInformation($"{expiredAuctions.Count} auctions expired.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CheckAndExpireAuctionsAsync");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndExpireAuctionsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking and expiring auctions.");
                }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
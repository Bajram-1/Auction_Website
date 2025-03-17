using Auction_Website.BLL.IServices;
using Auction_Website.BLL.Services;
using Auction_Website.BLL.Services.Singletons;
using Microsoft.Extensions.DependencyInjection;

namespace Auction_Website.BLL
{
    public static class BLLStartup
    {
        public static void RegisterBLLServices(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerService, LoggerService>();
            services.AddScoped<IAuctionService, AuctionService>();
            services.AddHostedService<AuctionClosingService>();
        }
    }
}
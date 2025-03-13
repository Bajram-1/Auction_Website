using Auction_Website.DAL.IRepositories;
using Auction_Website.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auction_Website.DAL
{
    public static class DALStartup
    {
        public static void RegisterDALServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IAuctionRepository, AuctionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
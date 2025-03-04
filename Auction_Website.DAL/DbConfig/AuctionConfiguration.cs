using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Auction_Website.DAL.Entities;

namespace Auction_Website.DAL.DbConfig
{
    public class AuctionConfiguration : IEntityTypeConfiguration<Auction>
    {
        public void Configure(EntityTypeBuilder<Auction> builder)
        {
            builder.ToTable("Auctions");

            builder.HasKey(a => a.AuctionId);

            builder.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Description)
                .IsRequired();

            builder.Property(a => a.StartingPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(a => a.StartTime)
                .IsRequired();

            builder.Property(a => a.EndTime)
                .IsRequired();

            builder.HasMany(a => a.Bids)
                .WithOne(b => b.Auction)
                .HasForeignKey(b => b.AuctionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
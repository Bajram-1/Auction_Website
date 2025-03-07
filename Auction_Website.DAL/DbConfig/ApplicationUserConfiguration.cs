using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Auction_Website.DAL.Entities;

namespace Auction_Website.DAL.DbConfig
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("Users");

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.WalletBalance)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.HasMany(u => u.AuctionsCreated)
                .WithOne(a => a.CreatedByUser)
                .HasForeignKey(a => a.CreatedByUserId);

            builder.HasMany(u => u.Bids)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId);

            builder.HasMany(u => u.TransfersSent)
                .WithOne(t => t.FromUser)
                .HasForeignKey(t => t.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.TransfersReceived)
                .WithOne(t => t.ToUser)
                .HasForeignKey(t => t.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
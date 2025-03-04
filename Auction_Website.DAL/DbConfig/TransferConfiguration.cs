using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Auction_Website.DAL.Entities;

namespace Auction_Website.DAL.DbConfig
{
    public class TransferConfiguration : IEntityTypeConfiguration<Transfer>
    {
        public void Configure(EntityTypeBuilder<Transfer> builder)
        {
            builder.ToTable("Transfers");

            builder.HasKey(t => t.TransferId);

            builder.Property(t => t.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(t => t.TransferDate)
                .IsRequired();

            builder.Property(t => t.Reason)
                .HasMaxLength(200);

            builder.HasOne(t => t.FromUser)
                .WithMany(u => u.TransfersSent)
                .HasForeignKey(t => t.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.ToUser)
                .WithMany(u => u.TransfersReceived)
                .HasForeignKey(t => t.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
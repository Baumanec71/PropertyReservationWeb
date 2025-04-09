using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Configurations
{
    public class BonusTransactionConfiguration : IEntityTypeConfiguration<BonusTransaction>
    {
        public void Configure(EntityTypeBuilder<BonusTransaction> builder)
        {
            builder
                .ToTable("BonusTransactions")
                .HasKey(bt => bt.Id);

            builder
                .Property(bt => bt.Description)
                .HasMaxLength(500)
                .IsRequired();

            builder
                .Property(bt => bt.DateCreate)
                .IsRequired();

            builder
                .Property(r => r.IsCalculated)
                .IsRequired();

            builder
                .Property(bt => bt.Type)
                .HasMaxLength(50)
                .IsRequired();

            builder
                .HasOne(bt => bt.User)
                .WithMany(u => u.BonusTransactions)
                .HasForeignKey(bt => bt.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(bt => bt.Review)
                .WithMany(r => r.BonusTransactions)
                .HasForeignKey(bt => bt.ReviewId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder
                .HasOne(bt => bt.Advertisement)
                .WithMany(a => a.BonusTransactions)
                .HasForeignKey(bt => bt.AdvertisementId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

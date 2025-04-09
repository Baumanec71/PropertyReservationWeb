using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder
                .ToTable("Reviews")
                .HasKey(r => r.Id);

            builder
                .Property(r => r.TheQualityOfTheTransaction)
                .IsRequired();

            builder
                .Property(r => r.Comment)
                .HasMaxLength(1000)
                .IsRequired();

            builder
                .Property(r => r.DateOfCreation)
                .IsRequired();

            builder
                .Property(r => r.StatusDel)
                .IsRequired();

            builder
                .Property(r => r.IsCalculated)
                .IsRequired();

            builder
                .HasOne(r => r.RentalRequest)
                .WithMany(rr => rr.Reviews)
                .HasForeignKey(r => r.IdNeedRentalRequest)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            //builder
            //    .HasOne(bt => bt.Review)
            //    .WithMany(r => r.BonusTransactions)
            //    .HasForeignKey<BonusTransaction>(bt => bt.ReviewId)
            //    .IsRequired(false)
            //    .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
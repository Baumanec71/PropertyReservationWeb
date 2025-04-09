using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Configurations
{
    public class RentalRequestConfiguration : IEntityTypeConfiguration<RentalRequest>
    {
        public void Configure(EntityTypeBuilder<RentalRequest> builder)
        {
            builder
                .ToTable("RentalRequests")
                .HasKey(r => r.Id);

            builder
                .Property(r => r.ApprovalStatus)
                .HasConversion<int>()
                .IsRequired();

            builder
                .Property(r => r.PaymentActiveId);

            builder
                .Property(x => x.BookingStartDate)
                .HasColumnType("timestamp with time zone")
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                .IsRequired();

            builder
                .Property(x => x.BookingFinishDate)
                .HasColumnType("timestamp with time zone")
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                .IsRequired();

            builder
                .Property(x => x.DataChangeStatus)
                .HasColumnType("timestamp with time zone")
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            builder
                .Property(r => r.DeleteStatus)
                .IsRequired();

            builder
                .HasOne(r => r.User)
                .WithMany(u => u.RentalRequests)
                .HasForeignKey(r => r.IdAuthorRentalRequest)
                .IsRequired();

            builder
                .HasOne(r => r.Advertisement)
                .WithMany(a => a.RentalRequests)
                .HasForeignKey(r => r.IdNeedAdvertisement)
                .IsRequired();

            builder
                .HasMany(r => r.Reviews)
                .WithOne()
                .HasForeignKey("IdNeedRentalRequest")
                .OnDelete(DeleteBehavior.Cascade);

            builder
               .HasMany(r => r.Payments)
               .WithOne(p => p.RentalRequest)
               .HasForeignKey("RentalRequestId")
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

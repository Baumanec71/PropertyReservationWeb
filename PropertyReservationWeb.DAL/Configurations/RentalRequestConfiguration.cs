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
                .Property(r => r.PaymentActiveDepositId);

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

            builder.Property(x => x.FixedPrepaymentAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.FixedDepositAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder
                .Property(r => r.DeleteStatus)
                .IsRequired();

            builder
                .Property(r => r.IsCalculated)
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

            builder
                .Property(r => r.CheckInTime)
                .HasColumnType("time")
                .IsRequired();

            builder
                .Property(r => r.CheckOutTime)
                .HasColumnType("time")
                .IsRequired();

            builder
                .Property(r => r.IsBeforePhotosUploaded)
                .IsRequired();

            builder
                .Property(r => r.IsAfterPhotosUploaded)
                .IsRequired();
            builder
                .Property(r => r.IsPhotoSkippedByLandlord)
                .IsRequired();

            builder
                .HasOne(r => r.ReservationChangeRequest)
                .WithOne()
                .HasForeignKey<RentalRequest>(r => r.ReservationChangeRequestId)
                .OnDelete(DeleteBehavior.SetNull); // или .Restrict в зависимости от логики
        }
    }
}

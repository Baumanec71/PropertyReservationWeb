using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Configurations
{
    public class ReservationChangeRequestConfiguration : IEntityTypeConfiguration<ReservationChangeRequest>
    {
        public void Configure(EntityTypeBuilder<ReservationChangeRequest> builder)
        {
            builder
                .ToTable("ReservationChangeRequests")
                .HasKey(r => r.Id);

            builder
                .Property(r => r.NewStartDate)
                .HasColumnType("timestamp with time zone")
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                .IsRequired();

            builder
                .Property(r => r.NewFinishDate)
                .HasColumnType("timestamp with time zone")
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                .IsRequired();

            builder
                .Property(r => r.NewFixedPrepaymentAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder
                .Property(r => r.NewFixedDepositAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder
                .Property(r => r.Status)
                .HasConversion<int>()
                .IsRequired();

            builder
                .Property(r => r.RequestedByUserId)
                .IsRequired();

            builder
                .Property(r => r.CreateDate)
                .HasColumnType("timestamp with time zone")
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                .IsRequired();

            // Связь с RentalRequest (один к одному)
            builder
                .HasOne(r => r.RentalRequest)
                .WithOne(rq => rq.ReservationChangeRequest)
                .HasForeignKey<ReservationChangeRequest>(r => r.RentalRequestId)
                .OnDelete(DeleteBehavior.Cascade); // или .Restrict — зависит от логики

            // Связь с NewPayments (один ко многим)
            builder
                .HasMany(r => r.NewPayments)
                .WithOne()
                .HasForeignKey("ReservationChangeRequestId")
                .OnDelete(DeleteBehavior.Cascade); // добавь это поле в PaymentRentalRequest
        }
    }
}

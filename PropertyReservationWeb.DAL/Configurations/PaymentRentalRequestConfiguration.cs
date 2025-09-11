using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Configurations
{
    public class PaymentRentalRequestConfiguration : IEntityTypeConfiguration<PaymentRentalRequest>
    {
        public void Configure(EntityTypeBuilder<PaymentRentalRequest> builder)
        {
            builder
                .ToTable("PaymentRentalRequests")
                .HasKey(p => p.Id);
            builder
                .Property(p => p.Id)
                .HasColumnType("text")
                .IsRequired();

            builder
                .HasOne(p => p.RentalRequest)
                .WithMany(r => r.Payments)
                .HasForeignKey("RentalRequestId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder
                .Property(p => p.IsPayment)
                .IsRequired();

            builder
                .Property(p => p.Url)
                .IsRequired();

            builder
                .Property(p => p.Status)
                .HasConversion<int>()
                .IsRequired();

            builder
                .Property(x => x.PaymentDate)
                .HasColumnType("timestamp with time zone")
                .HasConversion(
                    v => v.HasValue ? v.Value.ToUniversalTime() : (DateTime?)null,
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null);

            builder
                .Property(x => x.CreateDate)
                .HasColumnType("timestamp with time zone")
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                .IsRequired();

            builder
                .HasOne(p => p.ReservationChangeRequest)
                .WithMany()
                .HasForeignKey(p => p.ReservationChangeRequestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

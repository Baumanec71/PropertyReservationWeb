using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Configurations
{
    public class ConflictConfiguration : IEntityTypeConfiguration<Conflict>
    {
        public void Configure(EntityTypeBuilder<Conflict> builder)
        {
            builder.HasKey(c => c.Id);

            builder.HasOne(c => c.RentalRequest)
                .WithMany()
                .HasForeignKey(c => c.RentalRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.CreatedByUser)
                .WithMany()
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.ResolvedByAdmin)
                .WithMany()
                .HasForeignKey(c => c.ResolvedByAdminId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(c => c.Description)
                .HasMaxLength(1000); // Ограничение на описание (если оно есть)

            builder.Property(c => c.Status)
                .HasConversion<int>()
                .IsRequired();

            builder
                .Property(x => x.DateCreated)
                .HasColumnType("timestamp with time zone")
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                .IsRequired();

            builder
                .Property(x => x.DateResolved)
                .HasColumnType("timestamp with time zone")
                .HasConversion(
                    v => v == null ? (DateTime?)null : v.Value.ToUniversalTime(),
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null);
        }
    }
}

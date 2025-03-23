using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Configurations
{
    public class ApprovalRequestConfiguration : IEntityTypeConfiguration<ApprovalRequest>
    {
        public void Configure(EntityTypeBuilder<ApprovalRequest> builder)
        {
            builder
                .ToTable("ApprovalRequests")
                .HasKey(r => r.Id);

            builder
                .Property(r => r.Status)
                .HasConversion<int>() 
                .IsRequired();

            builder
                .HasOne(r => r.User)
                .WithMany(t => t.ApprovalRequests)
                .HasForeignKey(r => r.IdUserAdmin)
                .IsRequired();

            builder
                .HasOne(t => t.Advertisement)
                .WithMany(r => r.ApprovalRequests)
                .HasForeignKey(t => t.IdAdvertisement)
                .IsRequired();

            builder
                .Property(x => x.DateCreate)
                .HasColumnType("timestamp with time zone")
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                .IsRequired();

            builder
                .Property(x => x.DateChange)
                .HasColumnType("timestamp with time zone")
                .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                .IsRequired();
        }
    }
}

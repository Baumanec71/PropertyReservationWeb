using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Configurations
{
    public class BookingPhotoConfiguration : IEntityTypeConfiguration<BookingPhoto>
    {
        public void Configure(EntityTypeBuilder<BookingPhoto> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder.Property(x => x.ValuePhoto)
                .IsRequired();

            builder.Property(x => x.DeleteStatus)
                .IsRequired();

            builder.Property(x => x.Before)
                .IsRequired();

            builder
                .HasOne(r => r.RentalRequest)
                .WithMany(t => t.BookingPhotos)
                .HasForeignKey(r => r.IdRentalRequest);

            builder
                .Property(x => x.DateCreate)
                .HasColumnType("timestamp with time zone")
                .HasConversion(
        v => v.ToUniversalTime(),
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        }
    }
}

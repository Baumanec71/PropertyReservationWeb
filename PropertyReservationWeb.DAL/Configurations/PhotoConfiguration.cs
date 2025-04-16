using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Configurations
{
    public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
    {
        public void Configure(EntityTypeBuilder<Photo> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder.Property(x => x.ValuePhoto)
                .IsRequired();


            builder.Property(x => x.DeleteStatus)
                .IsRequired();

            builder
                .HasOne(r => r.Advertisement)
                .WithMany(t => t.Photos)
                .HasForeignKey(r => r.IdAdvertisement);

            builder
                .Property(x => x.DateCreate)
                .HasColumnType("timestamp with time zone")
                .HasConversion(
        v => v.ToUniversalTime(),
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        }
    }
}

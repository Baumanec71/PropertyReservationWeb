using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Configurations
{
    public class AdvertisementAmenityConfiguration : IEntityTypeConfiguration<AdvertisementAmenity>
    {
        public void Configure(EntityTypeBuilder<AdvertisementAmenity> builder)
        {
            builder
                .ToTable("AdvertisementAmenities")
                .HasKey(x => x.Id);

            builder
                .HasOne(r => r.Advertisement)
                .WithMany(t => t.AdvertisementAmenities)
                .HasForeignKey(r => r.IdAdvertisement);

            builder
                .HasOne(r => r.Amenity)
                .WithMany(t => t.AdvertisementAmenity)
                .HasForeignKey(t => t.IdAmenity);
        }
    }
}

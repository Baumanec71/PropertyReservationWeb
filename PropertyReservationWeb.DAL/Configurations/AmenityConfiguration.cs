using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PropertyReservationWeb.Domain.Models;
using PropertyReservationWeb.Domain.Enum;

namespace PropertyReservationWeb.DAL.Configurations
{
    public class AmenityConfiguration : IEntityTypeConfiguration<Amenity>
    {
        public void Configure(EntityTypeBuilder<Amenity> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(x => x.AmenityType)
                .HasConversion<int>()
                .IsRequired();

            builder
                .HasMany(r => r.AdvertisementAmenity)
                .WithOne(t => t.Amenity)
                .HasForeignKey(t => t.IdAdvertisement);

            builder.HasData(
                Enum.GetValues(typeof(AmenityType))
                .Cast<AmenityType>()
                .Select(amenityType => new Amenity
                {
                    Id = (long)(amenityType+1),
                    AmenityType = amenityType
                })
                .ToArray()
);
        }
    }
}

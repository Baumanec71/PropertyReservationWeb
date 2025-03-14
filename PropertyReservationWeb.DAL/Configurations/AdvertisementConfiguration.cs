using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Configurations
{
    public class AdvertisementConfiguration : IEntityTypeConfiguration<Advertisement>
    {
        public void Configure(EntityTypeBuilder<Advertisement> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Description)
                .HasMaxLength(250)
                .IsRequired();

            builder
                .Property(x => x.ApartmentNumber)
                .HasMaxLength(50);
            
            builder
                .Property(x => x.RentalPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.FixedPrepaymentAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();


            builder
                .Property(x => x.DateCreate)
                .HasColumnType("timestamp with time zone")
                .HasConversion(
                 v => v.ToUniversalTime(),
                 v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                .IsRequired();

            builder
                .Property(x => x.AdressCoordinates)
    .           HasColumnType("geometry")
                .HasDefaultValueSql("ST_GeomFromText('POINT(0 0)', 4326)")
                .IsRequired();

            builder.Property(x => x.ObjectType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.TotalArea)
                .IsRequired();

            builder.Property(x => x.NumberOfRooms)
                .IsRequired();

            builder.Property(x => x.NumberOfBeds)
                .IsRequired();

            builder.Property(x => x.NumberOfBathrooms)
                .IsRequired();

            builder.Property(x => x.Rating)
                .IsRequired();

            builder.Property(x => x.NumberOfPromotionPoints)
                .IsRequired();

            builder.Property(x => x.ConfirmationStatus)
                .IsRequired();

            builder.Property(x => x.DeletionStatus)
                .IsRequired();

            builder
                .HasOne(r => r.User)
                .WithMany(t => t.Advertisements)
                .HasForeignKey(r => r.IdAuthor)
                .IsRequired();

            builder
                .HasMany(r => r.AdvertisementAmenities)
                .WithOne(t => t.Advertisement)
                .HasForeignKey(t => t.IdAdvertisement)
                .IsRequired();

        }
    }
}

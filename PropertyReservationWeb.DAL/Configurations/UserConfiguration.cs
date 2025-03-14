using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyReservationWeb.Domain.Enum;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .HasData(new User() {
                    Id = 1,
                    Password = "123456",
                    Email = "admin@example.com",
                    Role = Role.Admin,
                    Name = "Andrey",
                    Status = false,
                    Rating = 0,
                    Balans = 0,
                    Avatar = null,
                    PhoneNumber = "89992341221",
                    DateOfRegistration = DateTime.UtcNow,
                });

            builder
                .Property(x => x.Password)
                .HasMaxLength(100)
                .IsRequired();

            builder
                .Property(x => x.Email)
                .HasMaxLength(100)
                .IsRequired();

            builder
                .Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Balans)
                   .IsRequired();

            builder.Property(x => x.Avatar)
                   .IsRequired(false);

            builder
                .Property(x => x.PhoneNumber)
                .HasMaxLength(20)
                .IsRequired();

            builder
                .Property(x => x.DateOfRegistration)
                .HasColumnType("timestamp with time zone")
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                .IsRequired();

            //builder.HasMany(x => x.RefreshTokens)
            //    .WithOne(rt => rt.User)
            //    .HasForeignKey(rt => rt.IdUser)
            //    .IsRequired();
        }
    }


}
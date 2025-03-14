using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");
            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.Token)
                   .HasMaxLength(512)
                   .IsRequired();

            builder.Property(rt => rt.Expires)
                   .IsRequired();

            builder.Property(rt => rt.Created)
                   .IsRequired();

            builder.Property(rt => rt.CreatedByIp)
                   .HasMaxLength(45) // для IPv6 достаточно длины 45 символов
                   .IsRequired(false);

            builder.Property(rt => rt.Revoked)
                   .IsRequired(false);

            builder.Property(rt => rt.RevokedByIp)
                   .HasMaxLength(45)
                   .IsRequired(false);

            //builder.HasOne(rt => rt.User)
            //       .WithMany(u => u.RefreshTokens)
            //       .HasForeignKey(rt => rt.IdUser)
            //       .IsRequired();
        }
    }
}

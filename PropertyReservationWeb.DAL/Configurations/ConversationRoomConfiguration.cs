using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Configurations
{
    public class ConversationRoomConfiguration : IEntityTypeConfiguration<ConversationRoom>
    {
        public void Configure(EntityTypeBuilder<ConversationRoom> builder)
        {
            builder
                .ToTable("ConversationRooms")
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder
                .HasOne(r => r.OneUser)
                .WithMany(t => t.ConversationRooms1)
                .HasForeignKey(r => r.IdOneUser);
            builder
                .HasOne(r => r.TwoUser)
                .WithMany(t => t.ConversationRooms2)
                .HasForeignKey(r => r.IdTwoUser);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using PropertyReservationWeb.Domain.Models;
using PropertyReservationWeb.DAL.Configurations;

namespace PropertyReservationWeb.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<AdvertisementAmenity> AdvertisementAmenities { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<ApprovalRequest> ApprovalRequests { get; set; }
        public DbSet<RentalRequest> RentalRequests { get; set; }
        public DbSet<PaymentRentalRequest> PaymentRentalRequests { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<BonusTransaction> BonusTransactions { get; set; }
        public DbSet<ReservationChangeRequest> ReservationChangeRequests {  get; set; }
        public DbSet<Conflict> Conflicts { get; set; }
        public DbSet<BookingPhoto> BookingPhotos { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ConversationRoom> ConversationRooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new AdvertisementConfiguration());
            modelBuilder.ApplyConfiguration(new RentalRequestConfiguration());
            modelBuilder.ApplyConfiguration(new ReviewConfiguration());
            modelBuilder.ApplyConfiguration(new PhotoConfiguration());
            modelBuilder.ApplyConfiguration(new AmenityConfiguration());
            modelBuilder.ApplyConfiguration(new AdvertisementAmenityConfiguration());
            modelBuilder.ApplyConfiguration(new ApprovalRequestConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentRentalRequestConfiguration());
            modelBuilder.ApplyConfiguration(new BonusTransactionConfiguration());
            modelBuilder.ApplyConfiguration(new BookingPhotoConfiguration());
            modelBuilder.ApplyConfiguration(new ReservationChangeRequestConfiguration());
            modelBuilder.ApplyConfiguration(new ConflictConfiguration());
            modelBuilder.ApplyConfiguration(new MessageConfiguration());
            modelBuilder.ApplyConfiguration(new ConversationRoomConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
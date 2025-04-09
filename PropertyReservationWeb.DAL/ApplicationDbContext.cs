using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PropertyReservationWeb.Domain.Models;
using PropertyReservationWeb.Domain.Enum;
using PropertyReservationWeb.DAL.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
namespace PropertyReservationWeb.DAL
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public ApplicationDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ApplicationDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "Server=localhost;Port=5433;Database=kursach;User ID=postgres;Password=Bobriss_71;Maximum Pool Size=500;";
            optionsBuilder
                .UseNpgsql(connectionString, o => o.UseNetTopologySuite())
                .UseLoggerFactory(CreateLoggerFactory())
                .EnableSensitiveDataLogging();
        }

        private ILoggerFactory CreateLoggerFactory() =>
            LoggerFactory.Create(builder => { builder.AddConsole(); });
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
        public DbSet<Message> Messages { get; set; } // 
        public DbSet<ConversationRoom> ConversationRooms { get; set; } ///

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

            modelBuilder.ApplyConfiguration(new MessageConfiguration());
            modelBuilder.ApplyConfiguration(new ConversationRoomConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
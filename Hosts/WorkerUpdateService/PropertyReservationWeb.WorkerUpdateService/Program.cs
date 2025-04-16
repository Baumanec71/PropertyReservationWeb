using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using PropertyReservationWeb.DAL;
using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.DAL.Repositories;
using PropertyReservationWeb.Domain.Helpers;
using PropertyReservationWeb.Domain.Models;
using PropertyReservationWeb.Service.Implementations;
using PropertyReservationWeb.Service.Interfaces;
using System.Net.NetworkInformation;

namespace PropertyReservationWeb.UpdateServicePassports
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Строка подключения 'DB_CONNECTION_STRING' не найдена. Проверьте файл .env или переменные окружения.");
            }

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options
                    .UseNpgsql(connectionString, o => o.UseNetTopologySuite())
                    .UseLoggerFactory(CreateLoggerFactory())
                    .EnableSensitiveDataLogging();
            });

            ILoggerFactory CreateLoggerFactory() => LoggerFactory.Create(builder => { builder.AddConsole(); });

            builder.Services.AddScoped<IBaseRepository<User>, UserRepository>();
            builder.Services.AddScoped<IBaseRepository<Advertisement>, AdvertisementRepository>();
            builder.Services.AddScoped<IBaseRepository<Amenity>, AmenityRepository>();
            builder.Services.AddScoped<IBaseRepository<AdvertisementAmenity>, AdvertisementAmenityRepository>();
            builder.Services.AddScoped<IAdvertisementAmenityRepository, AdvertisementAmenityRepository>();
            builder.Services.AddScoped<IBaseRepository<Photo>, PhotoRepository>();
            builder.Services.AddScoped<IPhotoRepository<Photo>, PhotoRepository>();
            builder.Services.AddScoped<IPhotoRepository<BookingPhoto>, BookingPhotoRepository>();
            builder.Services.AddScoped<IBaseRepository<ApprovalRequest>, ApprovalRequestRepository>();
            builder.Services.AddScoped<IBaseRepository<Review>, ReviewRepository>();
            builder.Services.AddScoped<IBaseRepository<BonusTransaction>, BonusTransactionRepository>();
            builder.Services.AddScoped<IBonusTransactionRepository, BonusTransactionRepository>();
            builder.Services.AddScoped<IBaseRepository<RentalRequest>, RentalRequestRepository>();
            builder.Services.AddScoped<IBaseRepository<PaymentRentalRequest>, PaymentRentalRequestRepository>();
            builder.Services.AddScoped<IBaseRepository<BookingPhoto>, BookingPhotoRepository>();
            builder.Services.AddScoped<IBaseRepository<ReservationChangeRequest>, ReservationChangeRequestRepository>();
            builder.Services.AddScoped<IBaseRepository<Conflict>, ConflictRepository>();

            builder.Services.AddHttpClient();
            builder.Services.Configure<BonusSettings>(builder.Configuration.GetSection("BonusSettings"));

            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IBonusTransactionService, BonusTransactionService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IAdvertisementService, AdvertisementService>();
            builder.Services.AddScoped<IRentalRequestService, RentalRequestService>();
            builder.Services.AddScoped<IAmenityService, AmenityService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IBookingPhotoService, BookingPhotoService>();
            
            builder.Services.AddScoped<IJwtProvider, JwtProvider>();

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var migration = db.Database.GetService<IMigrator>();
                migration.Migrate();
            }

            app.MapControllers();

            app.Run();
        }
    }
}






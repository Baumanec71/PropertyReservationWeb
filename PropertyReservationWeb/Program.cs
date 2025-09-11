using PropertyReservationWeb.DAL;
using PropertyReservationWeb.DAL.Interfaces;
using PropertyReservationWeb.DAL.Repositories;
using PropertyReservationWeb.Domain.Helpers;
using PropertyReservationWeb.Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PropertyReservationWeb.Service.Implementations;
using PropertyReservationWeb.Service.Interfaces;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddScoped<IConflictService, ConflictService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IBonusTransactionService, BonusTransactionService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IAdvertisementService, AdvertisementService>();
builder.Services.AddScoped<IRentalRequestService, RentalRequestService>();
builder.Services.AddScoped<IAmenityService, AmenityService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBookingPhotoService, BookingPhotoService>();

builder.Services.AddHttpClient();
builder.Services.Configure<BonusSettings>(builder.Configuration.GetSection("BonusSettings"));
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options
        .UseNpgsql(connectionString, o => o.UseNetTopologySuite())
        .UseLoggerFactory(CreateLoggerFactory())
        .EnableSensitiveDataLogging();
});

ILoggerFactory CreateLoggerFactory() =>
    LoggerFactory.Create(builder => { builder.AddConsole(); });

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IBonusTransactionService, BonusTransactionService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IAdvertisementService, AdvertisementService>();
builder.Services.AddScoped<IRentalRequestService, RentalRequestService>();
builder.Services.AddScoped<IAmenityService, AmenityService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();

builder.Services
    .AddAuthentication(options=> 
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })

    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtOptions");
        var secretKey = jwtSettings["Key"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["my-cookies"];

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new GeoJsonConverter());
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://yoomoney.ru", "https://nicesait71front.serveo.net"); //"https://localhost:5173",
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowCredentials();
    });
});

//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.ListenAnyIP(5000); // HTTP
//    options.ListenAnyIP(5001, listenOptions =>
//    {
//        listenOptions.UseHttps(); // HTTPS
//    });
//});
//builder.WebHost.UseUrls("http://0.0.0.0:5000");
//builder.WebHost.UseUrls("https://0.0.0.0:5001");

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var migration = db.Database.GetService<IMigrator>();
    migration.Migrate();
}

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
   // Secure = CookieSecurePolicy.Always,
    Secure = CookieSecurePolicy.None,

});

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

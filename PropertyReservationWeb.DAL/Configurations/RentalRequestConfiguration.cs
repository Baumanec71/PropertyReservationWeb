using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Configurations
{
    public class RentalRequestConfiguration : IEntityTypeConfiguration<RentalRequest>
    {
        public void Configure(EntityTypeBuilder<RentalRequest> builder)
        {
            // Настройка таблицы
            builder
                .ToTable("RentalRequests") // Имя таблицы в БД
                .HasKey(r => r.Id); // Первичный ключ

            // Настройка свойства Id
            builder
                .Property(r => r.Id)
                .ValueGeneratedOnAdd(); // Генерация идентификатора

            // Настройка ApprovalStatus
            builder
                .Property(r => r.ApprovalStatus)
                .HasConversion<int>() // Хранение enum в виде целого числа
                .IsRequired(); // Обязательно

            // Настройка свойств дат
            builder
                .Property(r => r.BookingStartDate)
                .IsRequired();

            builder
                .Property(r => r.BookingFinishDate)
                .IsRequired();

            builder
                .Property(r => r.DataChangeStatus)
                .IsRequired();

            // Настройка булевых свойств
            builder
                .Property(r => r.DeleteStatus)
                .IsRequired();

            builder
                .Property(r => r.RecipientsViewingStatus)
                .IsRequired();

            builder
                .Property(r => r.AuthorsViewingStatus)
                .IsRequired();

            // Связь с таблицей User (автор заявки)
            builder
                .HasOne(r => r.User)
                .WithMany(u => u.RentalRequests) // Один пользователь - много заявок
                .HasForeignKey(r => r.IdAuthorRentalRequest) // Внешний ключ
                .IsRequired(); // Обязательно

            // Связь с таблицей Advertisement (объявление)
            builder
                .HasOne(r => r.Advertisement)
                .WithMany(a => a.RentalRequests) // Одно объявление - много заявок
                .HasForeignKey(r => r.IdNeedAdvertisement) // Внешний ключ
                .IsRequired(); // Обязательно

            // Связь с таблицей Review (отзывы)
            builder
                .HasMany(r => r.Review)
                .WithOne() // Если связь "один-к-многим", укажите здесь ссылку в Review
                .HasForeignKey("RentalRequestId") // Внешний ключ
                .OnDelete(DeleteBehavior.Cascade); // Каскадное удаление отзывов при удалении заявки
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyReservationWeb.Domain.Models;

namespace PropertyReservationWeb.DAL.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            // Настройка таблицы
            builder
                .ToTable("Reviews") // Имя таблицы в базе данных
                .HasKey(r => r.Id); // Указание первичного ключа

            // Настройка свойства Id
            builder
                .Property(r => r.Id)
                .ValueGeneratedOnAdd(); // Генерация идентификатора

            // Настройка TheQualityOfTheTransaction
            builder
                .Property(r => r.TheQualityOfTheTransaction)
                .IsRequired(); // Обязательное поле

            // Настройка Comment
            builder
                .Property(r => r.Comment)
                .HasMaxLength(1000) // Ограничение длины строки (пример)
                .IsRequired(); // Обязательное поле

            // Настройка DateOfCreation
            builder
                .Property(r => r.DateOfCreation)
                .IsRequired(); // Обязательное поле

            // Настройка булевых свойств
            builder
                .Property(r => r.StatusDel)
                .IsRequired();

            builder
                .Property(r => r.IsTheLandlord)
                .IsRequired();

            builder
                .Property(r => r.RecipientsViewingStatus)
                .IsRequired();

            builder
                .Property(r => r.AuthorsViewingStatus)
                .IsRequired();

            // Связь с таблицей RentalRequest
            builder
                .HasOne(r => r.RentalRequest) // Связь с RentalRequest
                .WithMany(rr => rr.Review) // Один RentalRequest - много Review
                .HasForeignKey(r => r.IdNeedRentalRequest) // Внешний ключ
                .IsRequired() // Поле обязательно
                .OnDelete(DeleteBehavior.Cascade); // Каскадное удаление
        }
    }


}
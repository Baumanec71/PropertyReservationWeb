using PropertyReservationWeb.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyReservationWeb.Domain.Models
{
    public class RentalRequest
    {
        public long Id { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public bool DeleteStatus { get; set; } // Статус удаления
        public DateTime BookingStartDate { get; set; } // Дата начала бронирования
        public DateTime BookingFinishDate { get; set; } // Дата окончания бронирования
        public bool RecipientsViewingStatus { get; set; } // Статус просмотра получателями
        public bool AuthorsViewingStatus { get; set; } // Статус просмотра автора
        public DateTime DataChangeStatus { get; set; } // Статус изменения данных
        public long IdAuthorRentalRequest { get; set; } // Идентификатор автора заявки
        public User User { get; set; } // Автор заявки
        public long IdNeedAdvertisement { get; set; } // Идентификатор необходимого объявления
        public Advertisement Advertisement { get; set; } // Связанное объявление
        public List<Review> Review { get; set; } // Отзывы

        // Приватный конструктор для защиты от несанкционированного создания экземпляров
        public RentalRequest()
        {
        }

        // Конструктор для инициализации всех свойств
        public RentalRequest(
            long id,
            ApprovalStatus approvalStatus,
            bool deleteStatus,
            DateTime bookingStartDate,
            DateTime bookingFinishDate,
            bool recipientsViewingStatus,
            bool authorsViewingStatus,
            DateTime dataChangeStatus,
            long idAuthorRentalRequest,
            User user,
            long idNeedAdvertisement,
            Advertisement advertisement
        )
        {
            Id = id;
            ApprovalStatus = approvalStatus;
            DeleteStatus = deleteStatus;
            BookingStartDate = bookingStartDate;
            BookingFinishDate = bookingFinishDate;
            RecipientsViewingStatus = recipientsViewingStatus;
            AuthorsViewingStatus = authorsViewingStatus;
            DataChangeStatus = dataChangeStatus;
            IdAuthorRentalRequest = idAuthorRentalRequest;
            User = user;
            IdNeedAdvertisement = idNeedAdvertisement;
            Advertisement = advertisement;
            Review = new List<Review>(); // Инициализация пустого списка отзывов
        }
    }
}

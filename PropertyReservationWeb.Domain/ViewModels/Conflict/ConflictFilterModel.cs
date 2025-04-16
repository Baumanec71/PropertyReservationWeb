using PropertyReservationWeb.Domain.Enum;

namespace PropertyReservationWeb.Domain.ViewModels.Conflict
{
    public class ConflictFilterModel
    {
        public long? RentalRequestId { get; set; } // Фильтрация по ID заявки аренды (может быть null)
        public long? ResolvedByAdminId { get; set; } // Фильтрация по администратору, разрешившему конфликт (может быть null)
        public ConflictStatus? Status { get; set; } // Фильтрация по статусу конфликта (может быть null)
    }
}

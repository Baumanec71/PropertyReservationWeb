using PropertyReservationWeb.Domain.Enum;

namespace PropertyReservationWeb.Domain.Models
{
    public class Conflict
    {
        public long Id { get; set; }

        // FK → RentalRequests.Id
        public long RentalRequestId { get; set; }
        public RentalRequest RentalRequest { get; set; }

        // FK → Users.Id
        public long CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }

        // FK → Admins.Id (или Users.Id если админы — это те же пользователи)
        public long? ResolvedByAdminId { get; set; }
        public User ResolvedByAdmin { get; set; } = null!;

        // Дополнительный комментарий
        public string Description { get; set; } = string.Empty;

        // Статус конфликта
        public ConflictStatus Status { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateResolved { get; set; }
    }
}

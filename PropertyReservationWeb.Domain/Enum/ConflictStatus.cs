using System.ComponentModel.DataAnnotations;

namespace PropertyReservationWeb.Domain.Enum
{
    public enum ConflictStatus
    {
        [Display(Name = "Открыт")]
        Open = 1,

        [Display(Name = "На рассмотрении")]
        InReview = 2,

        [Display(Name = "Решён")]
        Resolved = 3,

        [Display(Name = "Отклонён")]
        Rejected = 4,
    }
}

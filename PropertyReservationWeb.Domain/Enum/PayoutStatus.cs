using System.ComponentModel.DataAnnotations;

namespace PropertyReservationWeb.Domain.Enum
{
    public enum PayoutStatus
    {
        [Display(Name = "Ожидание")]
        Pending = 0,

        [Display(Name = "Одобрено")]
        Approved = 1,

        [Display(Name = "Отклонено")]
        Rejected = 2,

        [Display(Name = "Выполнено")]
        Processed = 3
    }
}

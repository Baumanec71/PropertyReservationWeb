using System.ComponentModel.DataAnnotations;

namespace PropertyReservationWeb.Domain.Enum
{
    public enum ReservationChangeStatus
    {
        [Display(Name = "На рассмотрении")]
        Pending = 0,

        [Display(Name = "Одобрено")]
        Approved = 1,

        [Display(Name = "Отклонено")]
        Rejected = 2,

        [Display(Name = "Завершено")]
        Completed = 3
    }
}

using System.ComponentModel.DataAnnotations;

namespace PropertyReservationWeb.Domain.Enum
{
    public enum ApprovalStatus
    {
        [Display(Name = "На рассмотрении")]
        UnderСonsideration = 0,
        [Display(Name = "Одобрен")]
        Approved = 1,
        [Display(Name = "Отклонен")]
        Rejected = 2,
        [Display(Name = "Оплачен")]
        Paid = 3,
        [Display(Name = "Завершен успешно")]
        Completed = 4,
    }
}

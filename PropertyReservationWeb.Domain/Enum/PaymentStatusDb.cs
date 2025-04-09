using System.ComponentModel.DataAnnotations;

namespace PropertyReservationWeb.Domain.Enum
{
    public enum PaymentStatusDb
    {
        [Display(Name = "Не оплачен")]
        Pending = 0,

        [Display(Name = "Отклонен")]
        Canceled = 1,

        [Display(Name = "Оплачен")]
        Succeeded = 2,

        [Display(Name = "Возврат")]
        Refund = 3,
    }
}

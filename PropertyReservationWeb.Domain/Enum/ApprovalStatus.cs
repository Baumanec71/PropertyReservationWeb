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
        [Display(Name = "Внесена предоплата")]
        PaidPayment = 3,
        [Display(Name = "Внесен залог")]
        PaidDeposit = 4,
        [Display(Name = "Оплачен")]
        Paid = 5,
        [Display(Name = "Залог и предоплата внесены, но выбраны новые даты")]
        PaidButNewDate = 6,
        [Display(Name = "Бронь началась")]
        TheBookingHasStarted = 7,
        //[Display(Name = "Арендодатель не доволен")]
        //TheLandlordIsUnhappy = 8,
        //[Display(Name = "Посетитель не доволен")]
        //TheTenantIsUnhappy = 9,
        [Display(Name = "Участники недовольны")]
        TheUsersIsUnhappy = 8,
        [Display(Name = "Завершена успешно")]
        Completed = 9,
    }
}

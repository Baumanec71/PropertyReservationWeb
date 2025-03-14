using System.ComponentModel.DataAnnotations;

namespace PropertyReservationWeb.Domain.Enum
{
    public enum ObjectType
    {
        [Display(Name = "Квартира")]
        Apartment = 0,
        [Display(Name = "Дом")]
        House = 1,
        [Display(Name = "Комната")]
        Room = 2
    }
}

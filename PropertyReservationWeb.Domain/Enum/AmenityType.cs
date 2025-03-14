using System.ComponentModel.DataAnnotations;

namespace PropertyReservationWeb.Domain.Enum
{
    public enum AmenityType
    {
        // Ванная комната
        [Display(Name = "Душ")]
        Shower = 0,

        [Display(Name = "Ванная")]
        Bath = 1,

        // Доступность
        [Display(Name = "Можно с детьми")]
        ChildrenAllowed = 2,

        [Display(Name = "Можно с животными")]
        PetsAllowed = 3,

        // Бытовые удобства
        [Display(Name = "WiFi")]
        WiFi = 4,

        [Display(Name = "TV")]
        TV = 5,

        [Display(Name = "Посудомойка")]
        Dishwasher = 6,

        [Display(Name = "Стиральная машина")]
        Washer = 7,

        [Display(Name = "Холодильник")]
        Fridge = 8,

        [Display(Name = "Кондиционер")]
        Conditioner = 9,

        // Здание и инфраструктура
        [Display(Name = "Лифт")]
        Elevator = 10,

        [Display(Name = "Парковка")]
        ParkingSpace = 11,

        [Display(Name = "Консьерж")]
        Concierge = 12,

        // Локация
        [Display(Name = "Пляж")]
        Beach = 13,

        [Display(Name = "Парк")]
        Park = 14
    }
}

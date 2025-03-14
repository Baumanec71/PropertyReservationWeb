using PropertyReservationWeb.Domain.Enum;
using PropertyReservationWeb.Domain.ViewModels.Amenity;
using PropertyReservationWeb.Domain.ViewModels.Photo;
using System.ComponentModel.DataAnnotations;

namespace PropertyReservationWeb.Domain.ViewModels.Advertisement
{
    public class CreateAdvertisementViewModel
    {
        [Display(Name = "Тип объекта")]
        [Required(ErrorMessage = "Укажите тип объекта")]
        public ObjectType ObjectType { get; set; }

        [Display(Name = "Адрес")]
        [Required(ErrorMessage = "Укажите адрес")]
        [MinLength(5, ErrorMessage = "Адрес должен быть больше 5 символов")]
        [MaxLength(200, ErrorMessage = "Адрес должен быть меньше 200 символов")]
        public string AdressName { get; set; } = string.Empty;

        [Display(Name = "Номер квартиры")]
        [MaxLength(50, ErrorMessage = "Номер квартиры должен быть меньше 50 символов")]
        public string ApartmentNumber { get; set; } = string.Empty;

        [Display(Name = "Долгота")]
        [Required(ErrorMessage = "Координат нет")]
        public double Latitude { get; set; }
        [Display(Name = "Широта")]
        [Required(ErrorMessage = "Координат нет")]
        public double Longitude { get; set; }

        [Display(Name = "Описание")]
        [Required(ErrorMessage = "Укажите описание недвижимости")]
        [MinLength(20, ErrorMessage = "Описание должно содержать не менее 20 символов")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Общая площадь")]
        [Required(ErrorMessage = "Укажите общую площадь недвижимости")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Стоимость должна быть больше 0")]
        public uint TotalArea { get; set; }

        [Display(Name = "Стоимость")]
        [Required(ErrorMessage = "Укажите цену аренды")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Стоимость должна быть больше 0")]
        public decimal RentalPrice { get; set; }

        [Display(Name = "Предоплата")]
        [Required(ErrorMessage = "Укажите сумму предоплаты")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Сумма предоплаты должна быть больше 0")]
        public decimal FixedPrepaymentAmount { get; set; }

        [Display(Name = "Количество комнат")]
        [Required(ErrorMessage = "Укажите количество комнат")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Количество комнат должно быть больше 0")]
        public uint NumberOfRooms { get; set; }

        [Display(Name = "Количество спальных мест")]
        [Required(ErrorMessage = "Укажите количество спальных мест")]
        public uint NumberOfBeds { get; set; }

        [Display(Name = "Количество туалетных комнат")]
        [Required(ErrorMessage = "Укажите количество туалетных комнат")]
        public uint NumberOfBathrooms { get; set; }

        [Display(Name = "Логин пользователя")]
        [Required(ErrorMessage = "У вас нет email")]
        public string Login { get; set; } = string.Empty;

        public List<ObjectTypeOptionViewModel> types { get; set; } = new();

        [Display(Name = "Фотографии")]
        public List<CreatePhotoViewModel> CreatePhotos { get; set; } = new();

        [Display(Name = "Удобства")]
        public List<CreateAdvertisementAmenityViewModel> CreateAdvertisementAmenities { get; set; } = new();
    }
}

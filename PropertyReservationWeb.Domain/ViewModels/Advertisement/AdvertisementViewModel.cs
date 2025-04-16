using NetTopologySuite.Geometries;
using PropertyReservationWeb.Domain.ViewModels.Convenience;
using PropertyReservationWeb.Domain.ViewModels.Photo;
using System.ComponentModel.DataAnnotations;

namespace PropertyReservationWeb.Domain.ViewModels.Advertisement
{
    public class AdvertisementViewModel
    {
        public long Id { get; set; }

        [Display(Name = "Тип объекта")]
        [Required(ErrorMessage = "Укажите тип объекта")]
        public string ObjectType { get; set; } = string.Empty;

        [Display(Name = "Адрес")]
        [Required(ErrorMessage = "Укажите адрес")]
        [MinLength(5, ErrorMessage = "Адрес должен быть больше 5 символов")]
        [MaxLength(200, ErrorMessage = "Адрес должен быть меньше 200 символов")]
        public string AdressName { get; set; } = string.Empty;

        [Display(Name = "Номер квартиры")]
        [MinLength(1, ErrorMessage = "Номер квартиры должен содержать хотя бы 1 символ")]
        [MaxLength(50, ErrorMessage = "Номер квартиры должен быть меньше 50 символов")]
        public string ApartmentNumber { get; set; } = string.Empty;

        [Display(Name = "Координаты адреса")]
        [Required(ErrorMessage = "Координат нет")]
        public Point? AdressCoordinates { get; set; }

        [Display(Name = "Описание")]
        [Required(ErrorMessage = "Укажите описание недвижимости")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Общая площадь")]
        [Required(ErrorMessage = "Укажите общую площадь недвижимости")]
        public uint? TotalArea { get; set; }

        [Display(Name = "Стоимость")]
        [Required(ErrorMessage = "Укажите цену аренды")]
        public decimal? RentalPrice { get; set; }

        [Display(Name = "Рейтинг")]
        public double Rating { get; set; }

        [Display(Name = "Количество сделок")]
        public int NumberOfTransactions { get; set; }
        
        [Display(Name = "Статус подтверждения")]
        public bool? ConfirmationStatus { get; set; }

        [Display(Name = "Статус удаления")]
        public bool? DeletionStatus { get; set; }

        [Display(Name = "Дата создания")]
        public string DateCreate { get; set; } = string.Empty;

        [Display(Name = "Количество комнат")]
        [Required(ErrorMessage = "Укажите количество комнат")]
        public uint? NumberOfRooms { get; set; }

        [Display(Name = "Количество спальных мест")]
        [Required(ErrorMessage = "Укажите количество спальных мест")]
        public uint? NumberOfBeds { get; set; }

        [Display(Name = "Количество туалетных комнат")]
        [Required(ErrorMessage = "Укажите количество туалетных комнат")]
        public uint? NumberOfBathrooms { get; set; }

        [Display(Name = "ID автора")]
        public long? IdAuthor { get; set; }

        [Display(Name = "Фотографии")]
        public List<PhotoViewModel> Photos { get; set; } = [];

        [Display(Name = "Удобства")]
        public List<AmenityViewModel> Amenityes { get; set; } = new();
    }
}

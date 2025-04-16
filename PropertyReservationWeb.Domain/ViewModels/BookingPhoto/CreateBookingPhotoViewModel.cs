using System.ComponentModel.DataAnnotations;

namespace PropertyReservationWeb.Domain.ViewModels.BookingPhoto
{
    public class CreateBookingPhotoViewModel
    {
        [Display(Name = "Фотография")]
        [Required(ErrorMessage = "Добавьте фотографию")]
        public string ValuePhoto { get; set; } = string.Empty;
        public bool? DeleteStatus { get; set; }

        [Required(ErrorMessage = "Нет даты создания")]
        public DateTime DateCreate { get; set; }
    }
}

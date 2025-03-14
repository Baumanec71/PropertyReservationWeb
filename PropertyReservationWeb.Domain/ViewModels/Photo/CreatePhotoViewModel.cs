using System.ComponentModel.DataAnnotations;

namespace PropertyReservationWeb.Domain.ViewModels.Photo
{
    public class CreatePhotoViewModel
    {
        [Display(Name = "Фотография")]
        [Required(ErrorMessage = "Добавьте фотографию")]
        public string ValuePhoto { get; set; } = string.Empty;
    }
}

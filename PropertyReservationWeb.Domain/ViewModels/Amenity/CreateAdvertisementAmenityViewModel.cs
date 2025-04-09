using PropertyReservationWeb.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace PropertyReservationWeb.Domain.ViewModels.Amenity
{
    public class CreateAdvertisementAmenityViewModel
    {
        [Required(ErrorMessage = "Укажите тип удобства")]
        public AmenityType Amenity { get; set; }
        public string AmenityDisplay { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите статус удобства")]
        public bool IsActive { get; set; }
        public int? Value { get; set; }
    }
}

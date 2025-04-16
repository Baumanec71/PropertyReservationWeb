using System.ComponentModel.DataAnnotations;

namespace PropertyReservationWeb.Domain.ViewModels.RentalRequest
{
    public class CreateRentalRequestViewModel
    {
        [Required (ErrorMessage = "Укажите дату заезда")]
        [Display(Name = "Дата заезда")]

        public DateTime? BookingStartDate { get; set; } = null!;
        [Required(ErrorMessage = "Укажите дату выезда")]
        [Display(Name = "Дата выезда")]
        public DateTime? BookingFinishDate { get; set; } = null!;
        public long IdNeedAdvertisement { get; set; }
        public List<DateTime> BookedDates { get; set; } = new();
        [Required(ErrorMessage = "Укажите время заезда")]
        [Display(Name = "Время заезда")]
        public TimeSpan CheckInTime { get; set; }
        [Required(ErrorMessage = "Укажите время выезда")]
        [Display(Name = "Время выезда")]
        public TimeSpan CheckOutTime { get; set; }
        public decimal RentalPrice { get; set; }
        public bool IsPhotoSkippedByLandlord { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace PropertyReservationWeb.Domain.ViewModels.Review
{
    public class CreateReviewViewModel
    {
        [Required]
        [Range(0, 5, ErrorMessage = "Оценка должна быть в диапазоне от 0 до 5.")]
        public int TheQualityOfTheTransaction { get; set; }

        [Required]
        public string Comment { get; set; } = string.Empty;

        [Required]
        public long IdRental { get; set; }
    }
}

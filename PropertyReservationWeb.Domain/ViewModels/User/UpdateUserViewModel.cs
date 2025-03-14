using System.ComponentModel.DataAnnotations;

namespace PropertyReservationWeb.Domain.ViewModels.User
{
    public class UpdateUserViewModel
    {
        [MaxLength(20, ErrorMessage = "Имя должно иметь длину меньше 20 символов")]
        [MinLength(2, ErrorMessage = "Имя должно иметь больше 2 символов")]
        public string? Name { get; set; } = string.Empty;

        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = "Телефон введен некорректно")]
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Avatar { get; set; }
    }
}

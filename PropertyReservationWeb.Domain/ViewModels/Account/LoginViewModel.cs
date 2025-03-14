using System.ComponentModel.DataAnnotations;

namespace PropertyReservationWeb.Domain.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введите почту")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(20, ErrorMessage = "Почта должна иметь длину меньше 20 символов")]
        [MinLength(3, ErrorMessage = "Почта должна иметь длину больше 3 символов")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string? Password { get; set; }
    }
}

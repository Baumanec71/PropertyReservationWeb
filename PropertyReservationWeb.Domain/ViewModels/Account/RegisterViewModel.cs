using System.ComponentModel.DataAnnotations;

namespace PropertyReservationWeb.Domain.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Укажите почту")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(20, ErrorMessage = "Почта должна иметь длину меньше 20 символов")]
        [MinLength(3, ErrorMessage = "Почта должна иметь длину больше 3 символов")]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Укажите пароль")]
        [MinLength(6, ErrorMessage = "Пароль должен иметь длину больше 6 символов")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string? PasswordConfirm { get; set; }
    }
}

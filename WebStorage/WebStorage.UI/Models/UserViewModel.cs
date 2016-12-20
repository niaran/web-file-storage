using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStorage.UI.Models
{
    // Этот класс будет использоваться пока только для теста работы Identity
    public class UserViewModel
    {
        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Введите Логин.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Длина Логина должна быть от 3 до 20 символов.")]
        public string Name { get; set; }

        [Display(Name = "Е-мейл")]
        [Required(ErrorMessage = "Введите Е-мейл.")]
        [EmailAddress(ErrorMessage = "Неверный Е-мейл.")]
        public string Email { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Введите Пароль.")]
        [RegularExpression(@"^[a-zA-Z\d]{8,}$", ErrorMessage = "Пароль не меньше 8 знаков - цифр, букв и спец. символов.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [NotMapped]
        [Display(Name = "Подтверждение пароля")]
        [Required(ErrorMessage = "Введите подтверждение пароля.")]
        [Compare("Password", ErrorMessage = "Пароль и Подтверждение пароля не совпадают.")]
        public string ConfirmPassword { get; set; }
    }
}
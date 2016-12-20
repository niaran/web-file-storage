using System.ComponentModel.DataAnnotations;

namespace WebStorage.UI.Models
{
    public class LoginModel
    {
        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Введите Логин.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Длина Логина должна быть от 3 до 20 символов.")]
        public string Name { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Введите Пароль.")]
        [RegularExpression(@"^[a-zA-Z\d]{8,}$", ErrorMessage = "Пароль не меньше 8 знаков - цифр, букв и спец. символов.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public bool remember { get; set; }
    }
}
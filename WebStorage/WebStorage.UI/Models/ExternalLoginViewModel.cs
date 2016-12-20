using System.ComponentModel.DataAnnotations;

namespace WebStorage.UI.Models
{
    public class ExternalLoginViewModel
    {
        [Display(Name = "Е-мейл")]
        [EmailAddress(ErrorMessage = "Неверный Е-мейл.")]
        public string Email { get; set; }

        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Введите Логин.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Длина Логина должна быть от 3 до 20 символов.")]
        public string Name { get; set; }
    }
}
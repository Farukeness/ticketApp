using System.ComponentModel.DataAnnotations;

namespace ticketApp.ViewModel
{
    public class CreateUserModel
    {
        [Required]
        [Display(Name ="İsim")]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [Display(Name ="E posta")]
        public string Email { get; set; } = string.Empty;
        [Required]

        [DataType(DataType.Password)]
        [Display(Name ="Şifre")]
        public string Password { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        [Display(Name ="Şifre Doğrula")]
        [Compare(nameof(Password), ErrorMessage = "Paralolar Eşleşmiyor")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
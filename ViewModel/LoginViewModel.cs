using System.ComponentModel.DataAnnotations;
namespace ticketApp.ViewModel;
public class LoginViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name ="E-posta")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Display(Name ="Şifre")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Beni Hatırla")]
    public bool RememberMe { get; set; } = true;
}

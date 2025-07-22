using System.ComponentModel.DataAnnotations;

namespace ticketApp.ViewModel
{
    public class CreateUserModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password),ErrorMessage ="Paralolar Eşleşmiyor")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
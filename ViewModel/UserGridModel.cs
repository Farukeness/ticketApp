using System.ComponentModel.DataAnnotations;
using ticketApp.Models;

namespace ticketApp.ViewModel
{
    public class UserGridModel
    {
        public string? Id { get; set; }
        [Required(ErrorMessage ="İsim Girmek Zorunlu")]
        public string UserName { get; set; }
        [Required(ErrorMessage ="E-posta Girmek Zorunlu")]
        public string? Email { get; set; }
        [Required(ErrorMessage ="Rol seçimi yapılmalıdır")]
        public string? CurrentRole { get; set; }
        
    }
}
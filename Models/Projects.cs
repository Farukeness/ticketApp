using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ticketApp.Models
{
    public class Projects
    {
        public int Id { get; set; }
        [Required]
        [Display(Name ="İsim")]
        public string Name { get; set; } = string.Empty;
        [Required]
        [Display(Name ="Açıklama")]
        public string Description { get; set; } = string.Empty;
        [Required]
        [Display(Name ="Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }
        [Required]
        [Display(Name ="Oluşturan")]
        public string CreatedByUserId { get; set; } = string.Empty;
    }
}

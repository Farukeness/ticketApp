using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ticketApp.Models
{
    public class Projects
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; }= string.Empty;
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        
        public string CreatedByUserId { get; set; } = string.Empty;
    }
}

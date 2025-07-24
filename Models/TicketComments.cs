using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ticketApp.Models
{
    public class TicketComments
    {
        public int Id { get; set; }
        
        public int TicketId { get; set; }
        
        public Tickets Tickets { get; set; } = null!;
        [Required]
        
        public string CommentText { get; set; } = string.Empty;
        [Required]
        public DateTime CommentedAt { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }

        [Required]
        public string CommentedByUserId { get; set; } = string.Empty;
        [ForeignKey(nameof(CommentedByUserId))]
        public AppUser AppUser { get; set; } = null!;
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ticketApp.Models
{
    public class TicketAttachments
    {
        public int Id { get; set; }

        public int TicketId { get; set; }
        public Tickets Tickets { get; set; } = null!;

        [Required]
        public string FileName { get; set; } = string.Empty;
        [Required]
        public string FilePath { get; set; } = string.Empty;
        [Required]
        public string ContentType { get; set; } = string.Empty;
        [Required]
        public DateTime UploadedAt { get; set; }
        [Required]

        public string UploadedByUserId { get; set; } = string.Empty;
        [ForeignKey(nameof(AppUser))]
        public AppUser AppUser { get; set; } = null!;
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ticketApp.Enums;
namespace ticketApp.Models

{
    public class Tickets
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TicketType ticketType { get; set; }
        public TicketPriority ticketPriority { get; set; }
        public TicketStatus ticketStatus { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }


        [Required]
        
        public string CreatedByUserId { get; set; } = string.Empty;
        public AppUser CreatedByUser { get; set; } = null!;


        public string? AssignedToUserId { get; set; }
        
        public AppUser? AssignedToUser { get; set; } 
        public int ProjectId { get; set; }

}
}
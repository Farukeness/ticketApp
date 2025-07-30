using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ticketApp.Enums;
namespace ticketApp.Models

{
    public class Tickets
    {
        public int Id { get; set; }
        [Required]
        [Display(Name="Başlık")]
        public string Title { get; set; } = string.Empty;
        [Display(Name="Açıklama")]
        public string? Description { get; set; }
        [Display(Name="Tip")]
        public TicketType ticketType { get; set; }
        [Display(Name="Öncelik")]
        public TicketPriority ticketPriority { get; set; }
        [Display(Name="Durum")]
        public TicketStatus ticketStatus { get; set; }
        [Display(Name="Oluşturulma Zamanı")]
        
        public DateTime CreatedAt { get; set; }


        
        
        public string CreatedByUserId { get; set; } = string.Empty;
        public AppUser? CreatedByUser { get; set; }

        [Display(Name ="Atanan Geliştiriciler")]

        public ICollection<AppUser> AssignedToUsers { get; set; } = new List<AppUser>(); 
        
        public int ProjectId { get; set; }

}
}
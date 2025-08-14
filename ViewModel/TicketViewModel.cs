using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ticketApp.Enums;
using ticketApp.Models;
namespace ticketApp.ViewModel

{
    public class TicketViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Başlık")]
        [Required(ErrorMessage = "Başlık alanı girmek zorunludur")]
        public string Title { get; set; } = string.Empty;
        [Display(Name = "Açıklama")]
        [Required(ErrorMessage = "Açıklama girmek zorunludur")]
        public string? Description { get; set; }
        [Display(Name = "Tip")]
        public TicketType ticketType { get; set; }
        [Display(Name = "Öncelik")]
        public TicketPriority ticketPriority { get; set; }
        [Display(Name = "Durum")]
        public TicketStatus ticketStatus { get; set; }
        [Display(Name = "Oluşturulma Zamanı")]

        public DateTime CreatedAt { get; set; }

        [Display(Name ="Atanma Durumu")]
        public string? AssignmentControl { get; set; }

        [Display(Name ="Oluşturan Kişi")]
        public string CreatedByUserId { get; set; } = string.Empty;
        public AppUser? CreatedByUser { get; set; }

        [Display(Name = "Atanan Geliştiriciler")]

        public ICollection<AppUser> AssignedToUsers { get; set; } = new List<AppUser>();

        
        public int ProjectId { get; set; }
        [Display(Name = "Proje Türü")]
        [Required(ErrorMessage = "Proje Seçmelisiniz")]
        public string? ProjectName { get; set; }
        

}
}
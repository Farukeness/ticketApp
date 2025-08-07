using ticketApp.Models;

namespace ticketApp.ViewModel
{
    public class TicketListViewModel
    {
        public Tickets? Ticket { get; set; } 
        public List<AppUser> Developers { get; set; } = new List<AppUser>();
    }
}
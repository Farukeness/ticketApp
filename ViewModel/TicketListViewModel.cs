using ticketApp.Models;

namespace ticketApp.ViewModel
{
    public class TicketListViewModel
    {
        public List<Tickets> Tickets { get; set; } = new List<Tickets>();
        public List<AppUser> Developers { get; set; } = new List<AppUser>();
    }
}
using ticketApp.Models;

namespace ticketApp.ViewModel
{
    public class TicketDetailViewModel
    {
        public Tickets? ticket { get; set; }
        public IQueryable<TicketComments>? ticketComment { get; set; }
        public IQueryable<TicketAttachments>? ticketAttachments { get; set; }

        public List<AppUser>? Usernames { get; set; }
    }
}
using ticketApp.Models;
using ticketApp.ViewModels;

namespace ticketApp.ViewModel
{
    public class TicketDetailViewModel
    {
        public Tickets? ticket { get; set; }
        public IEnumerable<TicketComments>? ticketComment { get; set; }
        public IQueryable<TicketAttachments>? ticketAttachments { get; set; }

        public List<AppUser>? Usernames { get; set; }
        public IEnumerable<SimpleAppUserViewModel> DevNames { get; set; }
        public IEnumerable<CommentViewModel> Comments { get; set; }
    }
}
using Microsoft.AspNetCore.Identity;

namespace ticketApp.Models
{
    public class AppUser : IdentityUser
    {
        public ICollection<Tickets> AssignedTickets { get; set; } = new List<Tickets>();
    }
}
using Microsoft.AspNetCore.Identity;

namespace ticketApp.Models
{
    public class AppUser : IdentityUser
    {
        public string? name { get; set; }
    }
}
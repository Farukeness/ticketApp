using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ticketApp.Models;

namespace ticketApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}
        public DbSet<Tickets> Tickets { get; set; }
        public DbSet<TicketAttachments> TicketAttachments { get; set; }
        public DbSet<TicketComments> TicketComments { get; set; }
        public DbSet<Projects> Projects { get; set; }
    }
}
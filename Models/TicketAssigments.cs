namespace ticketApp.Models
{
    public class TicketAssigments
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public Tickets tickets { get; set; } = null!;


        public string AppUserId { get; set; } = string.Empty;
        public AppUser AppUser { get; set; } = null!;
    }
}
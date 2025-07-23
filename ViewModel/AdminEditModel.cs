namespace ticketApp.ViewModel
{
    public class AdminEditModel
    {
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public string? CurrentRole { get; set; }
        public List<string>? Roles { get; set; }
        public string? SelectedRole { get; set; }
    }
}
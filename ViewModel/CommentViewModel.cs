namespace ticketApp.ViewModel
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CommentedAt { get; set; }
        public string CommentedUser { get; set; }
    }
}
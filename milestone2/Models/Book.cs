namespace milestone2.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
        public List<WaitlistEntry> Waitlist { get; set; } = new();
    }

    public class WaitlistEntry
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
    }
}

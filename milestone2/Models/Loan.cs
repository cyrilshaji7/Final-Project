namespace milestone2.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public string PatronEmail { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime BorrowedAt { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsReturned { get; set; }
    }
}

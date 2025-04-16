namespace milestone2.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public DateTime ReservedAt { get; set; }

        public Book? Book { get; set; }
    }

}

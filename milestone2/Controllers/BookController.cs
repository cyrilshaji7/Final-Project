using Microsoft.AspNetCore.Mvc;
using milestone2.Data;
using milestone2.Models;
using milestone2.DTOs;

namespace milestone2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly AppDbContext _context;
        public BookController(AppDbContext context) => _context = context;

        [HttpGet("search")] // ?query=Harry
        public IActionResult SearchBooks([FromQuery] string? query = "")
        {
            var books = string.IsNullOrWhiteSpace(query)
                ? _context.Books.ToList()
                : _context.Books
                    .Where(b => b.Title.Contains(query) || b.Author.Contains(query) || b.Genre.Contains(query))
                    .ToList();

            return Ok(books);
        }

       

        [HttpPost("waitlist/{id}")]
        public IActionResult JoinWaitlist(int id, [FromBody] string userEmail)
        {
            var book = _context.Books.Find(id);
            if (book == null) return NotFound();
            _context.WaitlistEntries.Add(new WaitlistEntry { BookId = id, UserEmail = userEmail });
            _context.SaveChanges();
            return Ok("Added to waitlist");
        }

        [HttpPost("reserve/{id}")]
        public IActionResult ReserveBook(int id, [FromBody] ReserveDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email is required.");

            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);
            if (user == null)
                return NotFound("User not found");

            var book = _context.Books.Find(id);
            if (book == null)
                return NotFound("Book not found");

            if (!book.IsAvailable)
                return BadRequest("Book is not available");

            book.IsAvailable = false;

            _context.Loans.Add(new Loan
            {
                BookId = id,
                PatronEmail = user.Email,
                BorrowedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14),
                IsReturned = false
            });

            _context.SaveChanges();
            return Ok("Book reserved and loaned successfully.");
        }
        [HttpPost("add")]
        public IActionResult AddBook([FromBody] Book book)
        {
            if (string.IsNullOrWhiteSpace(book.Title) || string.IsNullOrWhiteSpace(book.Author))
                return BadRequest("Title and Author are required.");

            _context.Books.Add(book);
            _context.SaveChanges();
            return Ok("Book added successfully");
        }



    }
}

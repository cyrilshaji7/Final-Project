using Microsoft.AspNetCore.Mvc;
using milestone2.Data;
using milestone2.Models;

namespace milestone2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly AppDbContext _context;
        public LoanController(AppDbContext context) => _context = context;

        [HttpGet("patron/{email}")]
        public IActionResult GetLoans(string email)
        {
            var loans = _context.Loans.Where(l => l.PatronEmail == email && !l.IsReturned).ToList();
            return Ok(loans);
        }

        [HttpPost("renew/{id}")]
        public IActionResult RenewLoan(int id)
        {
            var loan = _context.Loans.Find(id);
            if (loan == null || loan.IsReturned) return NotFound();
            loan.DueDate = loan.DueDate.AddDays(7);
            _context.SaveChanges();
            return Ok("Loan renewed");
        }

        [HttpPost("return/{id}")]
        public IActionResult ReturnBook(int id)
        {
            var loan = _context.Loans.Find(id);
            if (loan == null) return NotFound();
            loan.IsReturned = true;
            _context.Books.Find(loan.BookId)!.IsAvailable = true;
            _context.SaveChanges();
            return Ok("Book returned");
        }

        [HttpPost("reserve/{id}")]
        public IActionResult ReserveBook(int id, [FromBody] int userId)
        {
            var book = _context.Books.Find(id);
            if (book == null) return NotFound("Book not found");
            if (!book.IsAvailable) return BadRequest("Book is not available");

            book.IsAvailable = false;

            _context.Loans.Add(new Loan
            {
                BookId = id,
                UserId = userId,
                BorrowedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14)
            });

            _context.SaveChanges();
            return Ok("Book reserved and loaned successfully.");
        }

    }
}

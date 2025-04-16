using Microsoft.AspNetCore.Mvc;
using milestone2.Data;
using milestone2.Models;

namespace milestone2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        public InventoryController(AppDbContext context) => _context = context;

        [HttpPost("add-book")]
        public IActionResult AddBook([FromBody] Book book)
        {
            _context.Books.Add(book);
            _context.SaveChanges();
            return Ok("Book added");
        }

        [HttpGet("top-requests")]
        public IActionResult MostRequested()
        {
            var top = _context.Books.OrderByDescending(b => b.Waitlist.Count).Take(5).ToList();
            return Ok(top);
        }

        [HttpPost("mark-damaged/{id}")]
        public IActionResult MarkDamaged(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null) return NotFound();
            book.IsAvailable = false;
            _context.SaveChanges();
            return Ok("Book marked damaged");
        }
    }

}

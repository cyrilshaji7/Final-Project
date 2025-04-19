using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using milestone2.Data;
using milestone2.DTOs;
using milestone2.Models;
using milestone2.Services;

namespace milestone2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _service;

        public NotificationController(AppDbContext context, INotificationService service)
        {
            _context = context;
            _service = service;
        }

        [HttpPost("due-reminder")]
        public IActionResult DueReminder([FromBody] string userEmail)
        {
            var loans = _context.Loans
                .Where(l => l.PatronEmail == userEmail && l.DueDate <= DateTime.Now.AddDays(2))
                .ToList();

            foreach (var loan in loans)
            {
                _context.Notifications.Add(new Notification
                {
                    UserEmail = userEmail,
                    Message = $"Reminder: '{_context.Books.Find(loan.BookId)?.Title}' is due on {loan.DueDate:dd-MMM}."
                });
            }

            _context.SaveChanges();
            return Ok("Reminders sent");
        }

        [HttpPost("late-fees")]
        public IActionResult ApplyLateFees()
        {
            var overdue = _context.Loans
                .Where(l => !l.IsReturned && l.DueDate < DateTime.Now)
                .ToList();

            foreach (var loan in overdue)
            {
                _context.Notifications.Add(new Notification
                {
                    UserEmail = loan.PatronEmail,
                    Message = $"Late fee applied for overdue book ID {loan.BookId}."
                });
            }

            _context.SaveChanges();
            return Ok("Late fee notices created");
        }

        [HttpGet("overdue-report")]
        public IActionResult OverdueReport()
        {
            var overdue = _context.Loans
                .Where(l => !l.IsReturned && l.DueDate < DateTime.Now)
                .ToList();

            return Ok(overdue);
        }

        // Allow only users with Admin role to send notifications manually
        [HttpPost("send")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Send([FromBody] NotificationDto dto)
        {
            // You could also verify here by checking user claims if needed
            await _service.SendAsync(dto.UserId, dto.Message);
            return Ok(new { Message = "Notification sent" });
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var result = await _service.GetUserNotifications(id);
            return Ok(result);
        }
    }
}

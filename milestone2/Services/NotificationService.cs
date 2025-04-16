using milestone2.Data;
using milestone2.Models;
using Microsoft.EntityFrameworkCore;

namespace milestone2.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;
        public NotificationService(AppDbContext context) => _context = context;

        public async Task SendAsync(int userId, string message)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");

            var notification = new Notification
            {
                UserEmail = user.Email,
                Message = message,
                SentAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetUserNotifications(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");

            return await _context.Notifications
                .Where(n => n.UserEmail == user.Email)
                .ToListAsync();
        }
    }
}

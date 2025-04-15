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
            var notification = new Notification { UserId = userId, Message = message };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public Task<List<Notification>> GetUserNotifications(int userId) =>
            _context.Notifications.Where(n => n.UserId == userId).ToListAsync();
    }
}

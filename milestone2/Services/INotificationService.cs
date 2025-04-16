using milestone2.Models;

namespace milestone2.Services
{
    public interface INotificationService
    {
        Task SendAsync(int userId, string message);
        Task<List<Notification>> GetUserNotifications(int userId);
    }


}

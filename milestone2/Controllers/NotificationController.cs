using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using milestone2.DTOs;
using milestone2.Services;

namespace milestone2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationController(INotificationService service) => _service = service;

        [Authorize(Roles = "Admin")]
        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] NotificationDto dto)
        {
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

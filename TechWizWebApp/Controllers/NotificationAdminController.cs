using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechWizWebApp.Interfaces;

namespace TechWizWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationAdminController : ControllerBase
    {
        private readonly INotificationAdmin _notificationAdmin;

        public NotificationAdminController(INotificationAdmin notificationAdmin)
        {
            _notificationAdmin = notificationAdmin;
        }

        [HttpGet]
        [Authorize(Roles ="admin")]
        [Route("admin_notification")]
        public async Task<IActionResult> GetAdminNotification( [FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string type = "")
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
            int.TryParse(idClaim, out int userId);

            var customPaging = await _notificationAdmin.GetAdminNotification(userId, type, pageNumber, pageSize);

            return Ok(customPaging);
        }

        [HttpPost]
        [Authorize(Roles = "admin, designer")]
        [Route("delete_notification")]
        public async Task<IActionResult> DeleteNotification([FromForm] int notificationId)
        {
            var customResult = await _notificationAdmin.DeleteNotification(notificationId);

            return Ok(customResult);
        }

        [HttpPost]
        [Authorize(Roles = "admin, designer")]
        [Route("mark_read_notification")]
        public async Task<IActionResult> MarkAsRead([FromForm] int notificationId)
        {
            var customResult = await _notificationAdmin.MarkAsReadNotification(notificationId);

            return Ok(customResult);
        }

    }
}

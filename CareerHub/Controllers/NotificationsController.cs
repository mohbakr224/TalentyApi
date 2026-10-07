using Application.DTOs.Request;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace CareerHub.Controllers
{
    [ApiController]
    [Route("api/v1/notifications")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IPermissionChecker _permissionChecker;

        public NotificationsController(
            INotificationService notificationService,
            IPermissionChecker permissionChecker)
        {
            _notificationService = notificationService;
            _permissionChecker = permissionChecker;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendNotificationRequest request)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            var canSend = await _permissionChecker.HasPermissionAsync(User, "notification.send");
            if (!canSend)
                return Forbid();

            var result = await _notificationService.SendNotificationAsync(request, userId);
            return Ok(new { success = true, data = result });
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications([FromQuery] int limit = 20, [FromQuery] int offset = 0, CancellationToken cancellationToken = default)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            var list = await _notificationService.GetUserNotificationsAsync(userId, limit, offset, cancellationToken);
            return Ok(new { success = true, data = list });
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(new { success = true, data = new { unreadCount = count } });
        }

        [HttpPost("{id:guid}/read")]
        public async Task<IActionResult> MarkAsRead([FromRoute] Guid id)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            var success = await _notificationService.MarkAsReadAsync(id, userId);
            if (!success)
                return NotFound(new { success = false, message = "Notification not found" });

            return Ok(new { success = true });
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(new { success = true });
        }

        private bool TryGetUserId(out Guid userId)
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out userId);
        }
    }
}


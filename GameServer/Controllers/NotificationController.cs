using GameServer.Data;
using GameServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController :ControllerBase
    {
        private readonly GameDbContext _context;

        public NotificationController(GameDbContext context)
        {
            _context = context;
        }

        //플레이어 알림 목록 조회
        [HttpGet("{playerId}")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications(int playerId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.PlayerId == playerId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return notifications;
        }

        //읽지 않은 알림 개수 조회
        [HttpGet("{playerId}/unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount(int playerId)
        {
            var count = await _context.Notifications
                .CountAsync(n => n.PlayerId == playerId && !n.IsRead);

            return count;
        }

        //알림생성(시스템용)
        [HttpPost]
        public async Task<ActionResult<Notification>> CreateNotification([FromBody] Notification notification)
        {
            notification.CreatedAt = DateTime.UtcNow;
            notification.IsRead = false;

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNotification), new { notificationId = notification.NotificationId }, notification);
        }

        //특정 알림 조회
        [HttpGet("detail/{notificationId}")]
        public async Task<ActionResult<Notification>> GetNotification (int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);

            if(notification == null)
            {
                return NotFound();
            }

            return notification;
        }

        //알림 읽음 처리
        [HttpPut("{notificationId}/read")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);

            if(notification == null)
            {
                return NotFound();
            }

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        //모든 알림 읽음 처리
        [HttpPut("{playerId}/read-all")]
        public async Task<IActionResult> MarkAllAsRead(int playerId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.PlayerId == playerId && !n.IsRead)
                .ToListAsync();

            foreach(var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        //만료된 알림 삭제
        [HttpDelete("expired")]
        public async Task<IActionResult> DeleteExpiredNotifications()
        {
            var expiredNotifications = await _context.Notifications
                .Where(n => n.ExpiresAt != null && n.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();

            _context.Notifications.RemoveRange(expiredNotifications);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

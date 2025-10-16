using GameServer.Data;
using GameServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly GameDbContext _context;

        public FriendController(GameDbContext context)
        {
            _context = context;
        }

        //GET: api/Friend/5 - 특정 플레이어의 친구 목록 조회
        [HttpGet("{playerId}")]
        public async Task<ActionResult<IEnumerable<Friend>>> GetFriends(int playerId)
        {
            var friends = await _context.Friends
                .Where(f => f.PlayerId == playerId && f.Status == "Accepted")
                .ToListAsync();

            return friends;
        }

        //POST: api/Friend - 친구 신청
        [HttpPost]
        public async Task<ActionResult<Friend>> SendFriendRequest(Friend friend)
        {
            friend.Status = "Pending";
            friend.CreatedAt = DateTime.UtcNow;

            _context.Friends.Add(friend);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFriend", new {id = friend.FriendshipId}, friend);
        }

        //PUT: api/Friend/5 - 친구 신청 수락/거절
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFriendStatus(int id, Friend friend)
        {
            if(id != friend.FriendshipId)
            {
                return BadRequest();
            }
            if(friend.Status == "Accepted")
            {
                friend.AcceptedAt = DateTime.UtcNow;
            }
            _context.Entry(friend).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!FriendExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        //DELETE: api/Friend/5 - 친구 삭제
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFriend(int id)
        {
            var friend = await _context.Friends.FindAsync(id);
            if(friend == null)
            {
                return NotFound();
            }
            _context.Friends.Remove(friend);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //Get: api/Friend/pending/5 - 대기 중인 친구 신청 조회
        [HttpGet("pending/{playerId}")]
        public async Task<ActionResult<IEnumerable<Friend>>> GetPendingRequest(int playerId)
        {
            var pendingRequest = await _context.Friends
                .Where(f => f.FriendPlayerId == playerId && f.Status == "Pending")
                .ToListAsync();

            return pendingRequest;
        }
     
        private bool FriendExists(int id)
        {
            return _context.Friends.Any(e => e.FriendshipId == id);
        }
    }
}

using GameServer.Data;
using GameServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRewardController : ControllerBase
    {
        private readonly GameDbContext _context;

        public UserRewardController(GameDbContext context)
        {
            _context = context;
        }

        //GET: api/UserReward
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserReward>>> GetUserReward()
        {
            return await _context.UserRewards.Include(ur => ur.Player).ToListAsync();
        }

        //GET: api/UserReward/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserReward>> GetUserReward(int id)
        {
            var userReward = await _context.UserRewards
                .Include(ur => ur.Player)
                .FirstOrDefaultAsync(ur => ur.RewardId == id);

            if(userReward == null)
            {
                return NotFound();
            }
            return userReward;
        }

        //GET: api/UserReward/player/5
        [HttpGet("player/{playerId}")]
        public async Task<ActionResult<IEnumerable<UserReward>>> GetPlayerRewards(int playerId)
        {
            var rewards = await _context.UserRewards
                .Where(ur => ur.PlayerId == playerId)
                .Include(ur => ur.Player)
                .OrderByDescending(ur => ur.AcquiredAt)
                .ToListAsync();

            return rewards;
        }

        //POST: api/UserReward
        [HttpPost]
        public async Task<ActionResult<UserReward>> PostUserReward(UserReward userReward)
        {
            _context.UserRewards.Add(userReward);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserReward", new { id = userReward.RewardId }, userReward);
        }

        //POST: api/UserReward/give-daily-reward/5
        [HttpPost("give-daily-reward/{playerId}")]
        public async Task<ActionResult<UserReward>> GiveDailyReward(int playerId)
        {
            var player = await _context.Players.FindAsync(playerId);
            if(player == null)
            {
                return NotFound("Player not found");
            }

            var today = DateTime.UtcNow.Date;
            var existingReward = await _context.UserRewards
                .Where(ur => ur.PlayerId == playerId &&
                        ur.RewardType == "Daily" &&
                        ur.AcquiredAt.Date == today)
                .FirstOrDefaultAsync();

            if(existingReward != null)
            {
                return BadRequest("Daily reward already claimed today");
            }

            var dailyReward = new UserReward
            {
                PlayerId = playerId,
                RewardType = "Daily",
                RewardName = "Daily Login Bonus",
                Quantity = 100,
                Description = "Daily login reward",
                AcquiredAt = DateTime.UtcNow,
                IsCollected = false
            };

            _context.UserRewards.Add(dailyReward);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserReward", new { id = dailyReward.RewardId }, dailyReward);
        }

        //PUT: api/UserReward/collect/5
        [HttpPut("collect/{id}")]
        public async Task<IActionResult> CollectReward(int id)
        {
            var userReward = await _context.UserRewards.FindAsync(id);
            if(userReward == null)
            {
                return NotFound();
            }

            if (userReward.IsCollected)
            {
                return BadRequest("Reward already collected");
            }

            userReward.IsCollected = true;
            _context.Entry(userReward).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!UserRewardExists(id))
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

        //DELETE: api/UserReward/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserReward(int id)
        {
            var userReward = await _context.UserRewards.FindAsync(id);
            if(userReward == null)
            {
                return NotFound();
            }

            _context.UserRewards.Remove(userReward);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserRewardExists(int id)
        {
            return _context.UserRewards.Any(e => e.RewardId == id);
        }
    }
}

using GameServer.Data;
using GameServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
        private readonly GameDbContext _context;

        public LeaderboardController(GameDbContext context)
        {
            _context = context;
        }

        //GET: api/Leaderboard/top/10 - 상위 랭킹 조회
        [HttpGet("top/{count}")]
        public async Task<ActionResult<IEnumerable<Player>>> GetTopPlayers(int count = 10)
        {
            var topPlayers = await _context.Players
                .OrderByDescending(p => p.Level)
                .ThenByDescending(p => p.Experience)
                .Take(count)
                .ToListAsync();

            return topPlayers;
        }

        //GET: api/Leaderboard/player/5 - 특정 플레이어 랭킹 조회
        [HttpGet("player/{playerId}")]
        public async Task<ActionResult<object>> GetPlayerRank (int playerId)
        {
            var player = await _context.Players.FindAsync(playerId);
            if(player == null)
            {
                return NotFound();
            }
            var rank = await _context.Players
                .CountAsync(p => p.Level > player.Level ||
                (p.Level == player.Level && p.Experience > player.Experience)) +1;

            return new
            {
                PlayerId = playerId,
                Rank = rank,
                Level = player.Level,
                Experience = player.Experience
            };
        }

        //GET: api/Leaderboard/level/5 - 특정 레벨의 플레이어들 조회
        [HttpGet("level/{level}")]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayersByLevel(int level)
        {
            var players = await _context.Players
                .Where(p => p.Level == level)
                .OrderByDescending(p => p.Experience)
                .ToListAsync();

            return players;
        }

        //PUT: api/Leaderboard/5 - 플레이어 경험치 업데이트
        [HttpPut("{playerId}")]
        public async Task<IActionResult> UpdateExperience(int playerId, [FromBody] int newExperience)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
            {
                return NotFound();
            }

            player.Experience = newExperience;
            player.LastLoginAt = DateTime.UtcNow;

            if(player.Experience >= player.Level * 1000)
            {
                player.Level++;
            }
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

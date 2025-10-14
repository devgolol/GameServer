using GameServer.Data;
using GameServer.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameLogController : ControllerBase
    {
        public readonly GameDbContext _context;

        public GameLogController(GameDbContext context)
        {
            _context = context;
        }

        //GET: api/GameLog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameLog>>> GetGameLogs()
        {
            return await _context.GameLogs.ToListAsync();
        }

        //GET: api/GameLog/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameLog>> GetGameLog(int id)
        {
            var gameLog = await _context.GameLogs.FindAsync(id);

            if(gameLog == null)
            {
                return NotFound();
            }
            return gameLog;
        }

        //GET: api/GameLog/player/5 - 특정 플레이어 로그만 조회
        [HttpGet("player/{playerId}")]
        public async Task<ActionResult<IEnumerable<GameLog>>> GetPlayerLogs(int playerId)
        {
            var logs = await _context.GameLogs
                .Where(log => log.PlayerId == playerId)
                .OrderByDescending(log => log.CreatedAt)
                .ToListAsync();

            return logs;
        }

        //POST: api/GameLog
        [HttpPost]
        public async Task<ActionResult<GameLog>> PostGameLog(GameLog gameLog)
        {
            //로그 생성시간을 현재시간으로 자동설정
            gameLog.CreatedAt = DateTime.UtcNow;

            _context.GameLogs.Add(gameLog);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGameLog", new { id = gameLog.LogId }, gameLog);
        }

        //PUT: api/GameLog/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGameLog(int id, GameLog gameLog)
        {
            if(id != gameLog.LogId)
            {
                return BadRequest();
            }

            _context.Entry(gameLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!GameLogExists(id))
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

        //DELETE: api/GameLog/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGameLog(int id)
        {
            var gameLog = await _context.GameLogs.FindAsync(id);
            if(gameLog == null)
            {
                return NotFound();
            }

            _context.GameLogs.Remove(gameLog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GameLogExists(int id)
        {
            return _context.GameLogs.Any(e => e.LogId == id);
        }
    }
}

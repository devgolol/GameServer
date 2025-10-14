using GameServer.Data;
using GameServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameSessionController : ControllerBase
    {
        private readonly GameDbContext _context;

        public GameSessionController(GameDbContext context)
        {
            _context = context;
        }

        //GET: api/GameSession
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameSession>>> GetGameSessions()
        {
            return await _context.GameSessions.ToListAsync();
        }

        //Get: api/GameSession/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameSession>> GetGameSession(int id)
        {
            var gameSession = await _context.GameSessions.FindAsync(id);

            if(gameSession == null)
            {
                return NotFound();
            }
            return gameSession;
        }

        //POST: api/GameSession
        [HttpPost]
        public async Task<ActionResult<GameSession>> PostGameSession(GameSession gameSession)
        {
            _context.GameSessions.Add(gameSession);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGameSession", new { id = gameSession.SessionId }, gameSession);
        }

        //PUT: api/GameSession/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGameSession(int id, GameSession gameSession)
        {
            if(id != gameSession.SessionId)
            {
                return BadRequest();
            }
            _context.Entry(gameSession).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameSessionExists(id))
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


        //DELETE: api/GameSession/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGameSession(int id)
        {
            var gameSession = await _context.GameSessions.FindAsync(id);

            if (gameSession == null)
            {
                return NotFound();
            }

            _context.GameSessions.Remove(gameSession);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GameSessionExists(int id)
        {
            return _context.GameSessions.Any(e => e.SessionId == id);
        }
    }
}

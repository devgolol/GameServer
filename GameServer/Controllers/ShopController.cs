using GameServer.Data;
using GameServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly GameDbContext _context;

        public ShopController(GameDbContext context)
        {
            _context = context;
        }

        //GET: api/Shop - 상점 아이템 목록 조회
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Shop>>> GetShopItems()
        {
            return await _context.Shops
                .Where(s => s.IsActive)
                .OrderBy(s => s.SortOrder)
                .ToListAsync();
        }

        //GET: api/Shop/5 - 특정 상점 아이템 조회
        [HttpGet("{id}")]
        public async Task<ActionResult<Shop>> GetShopItem(int id)
        {
            var shopItem = await _context.Shops.FindAsync(id);

            if (shopItem == null)
            {
                return NotFound();
            }

            return shopItem;
        }

        //POST: api/Shop - 새 상점 아이템 추가
        [HttpPost]
        public async Task<ActionResult<Shop>> PostShopItem(Shop shop)
        {
            shop.CreatedAt = DateTime.UtcNow;
            shop.UpdatedAt = DateTime.UtcNow;

            _context.Shops.Add(shop);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetShopItem", new { id = shop.ShopItemId }, shop);
        }

        //PUT: api/Shop/5 - 상점 아이템 수정
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShopItem(int id, Shop shop)
        {
            if(id != shop.ShopItemId)
            {
                return BadRequest();
            }
            shop.UpdatedAt = DateTime.UtcNow;
            _context.Entry(shop).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!ShopItemExists(id))
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

        //DELETE: api/Shop/5 - 상점 아이템 삭제
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShopItem(int id)
        {
            var shop = await _context.Shops.FindAsync(id);
            if (shop == null)
            {
                return NotFound();
            }
            _context.Shops.Remove(shop);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ShopItemExists(int id)
        {
            return _context.Shops.Any(e => e.ShopItemId == id);
        }
    }
}

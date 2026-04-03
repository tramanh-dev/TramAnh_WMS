using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TramAnh_WMS.Data;
using TramAnh_WMS.Models;

namespace TramAnh_WMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InventoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetInventories()
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .Include(i => i.Location)
                .ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Inventory>> GetInventories(int id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null) return NotFound();
            return inventory;
        }
        [HttpPost]
        public async Task<ActionResult<Inventory>> PostInventory(Inventory inventory)
        {
            _context.Inventories.Add(inventory);
            await _context.SaveChangesAsync();
            return Ok(inventory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutInventory(int id, Inventory inventory)
        {
            if (id != inventory.ProductId) return BadRequest();
            _context.Entry(inventory).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            // Tìm bản ghi tồn kho dựa trên ProductId ([Key])
            var inventory = await _context.Inventories.FindAsync(id);

            if (inventory == null)
            {
                return NotFound("Không tìm thấy sản phẩm này trong kho!");
            }
            _context.Inventories.Remove(inventory);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/Inventory/find?name=...
        [HttpGet("find")]
        public async Task<ActionResult> FindProductLocation(string name)
        {
            // Tìm kiếm thông minh: Trả về vị trí chính xác của món hàng
            var result = await _context.Inventories
                .Include(i => i.Product)
                .Include(i => i.Location)
                .Where(i => i.Product.Name.Contains(name)) 
                .Select(i => new {
                    TenSanPham = i.Product.Name,
                    SoLuongTrongKho = i.QuantityOnHand,
                    ViTriKe = $"{i.Location.Aisle}-{i.Location.Shelf}-{i.Location.Level}",
                    MaVịTri = i.Location.LocationCode 
                })
                .ToListAsync();

            if (result.Count == 0)
            {
                return NotFound($"Không tìm thấy '{name}' trong kho!");
            }

            return Ok(result);
        }
        // GET: api/Inventory/low-stock?threshold=20
        [HttpGet("low-stock")]
        public async Task<ActionResult> GetLowStock(int threshold = 20)
        {
            var lowStockItems = await _context.Inventories
                .Include(i => i.Product)
                .Where(i => i.QuantityOnHand < threshold)
                .Select(i => new
                {
                    TenSP = i.Product.Name,
                    SoLuongConLai = i.QuantityOnHand,
                    Khuyen = "Mặt hàng này sắp hết hãy nhập thêm hàng !"
                })
                .ToListAsync();

            return Ok(lowStockItems);
        }

        // GET: api/Inventory/total-report
        [HttpGet("total-report")]
        public async Task<ActionResult> GetTotalInventoryReport()
        {
            var report = await _context.Inventories
                .Include(i => i.Product)
                .GroupBy(i => i.Product.Name) 
                .Select(group => new {
                    ProductName = group.Key,
                    TotalStock = group.Sum(i => i.QuantityOnHand), // Cộng tổng tồn kho
                    LocationCount = group.Count() // Đếm xem nó nằm ở bao nhiêu vị trí
                })
                .ToListAsync();

            return Ok(report);
        }
    }
}

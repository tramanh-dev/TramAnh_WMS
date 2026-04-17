using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TramAnh_WMS.Data;
using TramAnh_WMS.Models;

namespace TramAnh_WMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderItemController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/OrderItem
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems()
        {
            return await _context.OrderItems
                .Include(o => o.Product)
                .ToListAsync();
        }

        // POST: api/OrderItem (Thêm món hàng vào đơn + check tồn kho)
        [HttpPost]
        public async Task<ActionResult<OrderItem>> PostOrderItem(OrderItem orderItem)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Kiểm tra Order tồn tại
                var order = await _context.Orders.FindAsync(orderItem.OrderId);
                if (order == null)
                {
                    return BadRequest(new { message = "Order không tồn tại!" });
                }

                // 2. Kiểm tra tồn kho
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.ProductId == orderItem.ProductId);

                if (inventory == null || inventory.QuantityAvailable < orderItem.QuantityOrdered)
                {
                    return BadRequest(new { message = "Không đủ hàng trong kho!" });
                }

                // 3. Trừ tồn kho (giữ chỗ)
                inventory.QuantityAvailable -= orderItem.QuantityOrdered;

                // 4. Lưu OrderItem
                _context.OrderItems.Add(orderItem);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(orderItem);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        // DELETE: api/OrderItem/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            var item = await _context.OrderItems.FindAsync(id);
            if (item == null) return NotFound();

            // Hoàn lại tồn kho khi xóa
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.ProductId == item.ProductId);

            if (inventory != null)
            {
                inventory.QuantityAvailable += item.QuantityOrdered;
            }

            _context.OrderItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
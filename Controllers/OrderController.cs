using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TramAnh_WMS.Data;
using TramAnh_WMS.Models;

namespace TramAnh_WMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.Include(o => o.Store).ToListAsync();
        }
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            order.Status = "New";
            order.TotalAmount = 0;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new { orderId = order.OrderId });
        }

        [HttpPut("ConfirmPicked/{id}")]
        public async Task<IActionResult> ConfirmPicked(int id, List<OrderItem> pickedItems)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.OrderId == id);

                if (order == null) return NotFound();

                foreach (var item in pickedItems)
                {
                    var dbItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == item.ProductId);
                    if (dbItem == null) continue;

                    // ❗ Check hợp lệ
                    if (item.QuantityPicked < 0 || item.QuantityPicked > dbItem.QuantityOrdered)
                    {
                        return BadRequest(new { message = $"Số lượng pick không hợp lệ cho sản phẩm {item.ProductId}" });
                    }

                    dbItem.QuantityPicked = item.QuantityPicked;

                    var inventory = await _context.Inventories
                        .FirstOrDefaultAsync(i => i.ProductId == item.ProductId);

                    if (inventory == null || inventory.QuantityOnHand < item.QuantityPicked)
                    {
                        return BadRequest(new { message = $"Kho không đủ hàng để xuất cho sản phẩm {item.ProductId}" });
                    }

                    inventory.QuantityOnHand -= item.QuantityPicked;
                }

                order.Status = "Picked";

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Đã xác nhận nhặt hàng xong!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }
    }
}
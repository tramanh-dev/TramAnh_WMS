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
            // Bắt đầu một Transaction để đảm bảo tính toàn vẹn dữ liệu
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Kiểm tra từng món hàng trong đơn
                foreach (var item in order.OrderItems)
                {
                    var inventory = await _context.Inventories
                        .FirstOrDefaultAsync(i => i.ProductId == item.ProductId);

                    if (inventory == null || inventory.QuantityAvailable < item.QuantityOrdered)
                    {
                        return BadRequest(new { message = $"Sản phẩm ID {item.ProductId} không đủ hàng trong kho!" });
                    }

                    // Giảm QuantityAvailable (giữ chỗ), nhưng CHƯA giảm QuantityOnHand (hàng vẫn nằm trong kho)
                    inventory.QuantityAvailable -= item.QuantityOrdered;
                }

                order.Status = "New";
                order.TotalAmount = 0; 

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

              
                await transaction.CommitAsync();

                return CreatedAtAction("GetOrders", new { id = order.OrderId }, order);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPut("ConfirmPicked/{id}")]
        public async Task<IActionResult> ConfirmPicked(int id, List<OrderItem> pickedItems)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();

            foreach (var item in pickedItems)
            {
                var dbItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == item.ProductId);
                if (dbItem != null)
                {
                    dbItem.QuantityPicked = item.QuantityPicked;

                  // Trừ QuantityOnHand
                    var inventory = await _context.Inventories.FirstOrDefaultAsync(i => i.ProductId == item.ProductId);
                    if (inventory != null)
                    {
                        inventory.QuantityOnHand -= item.QuantityPicked;
                    }
                }
            }

            order.Status = "Picked";
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xác nhận nhặt hàng xong!" });
        }
    }
}
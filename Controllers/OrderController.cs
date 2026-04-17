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
            return await _context.Orders.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> PostOrder(Order order)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                order.Status = "New";
                order.TotalAmount = 0;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { orderId = order.OrderId });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("ConfirmPicked/{id}")]
        public async Task<IActionResult> ConfirmPicked(int id, List<OrderItem> pickedItems)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = await _context.Orders
                  .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                  .FirstOrDefaultAsync(o => o.OrderId == id);

                if (order?.OrderItems == null)
                    return BadRequest("Order không có items");

                foreach (var item in pickedItems)
                {
                    var dbItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == item.ProductId);
                    if (dbItem == null) continue;

                    if (item.QuantityPicked < 0 || item.QuantityPicked > dbItem.QuantityOrdered)
                        return BadRequest($"Sai số lượng product {item.ProductId}");

                    var inventory = await _context.Inventories
                      .FirstOrDefaultAsync(i => i.ProductId == item.ProductId);

                    if (inventory == null || inventory.QuantityOnHand < item.QuantityPicked)
                        return BadRequest($"Không đủ kho product {item.ProductId}");

                    dbItem.QuantityPicked = item.QuantityPicked;
                    inventory.QuantityOnHand -= item.QuantityPicked;
                }

                order.Status = "Picked";

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "OK" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, ex.Message);
            }
        }
    }
}
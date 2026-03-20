using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TramAnh_WMS.Data;
using TramAnh_WMS.Models;

namespace TramAnh_WMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PickBatchesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PickBatchesController(AppDbContext context) { _context = context; }

        //  GOM TẤT CẢ ĐƠN HÀNG "NEW" THÀNH 1 BATCH MỚI
        [HttpPost("CreateBatch")]
        public async Task<ActionResult<PickBatch>> CreateBatch(int staffId)
        {
            // 1. Tìm các đơn hàng đang ở trạng thái "New" và chưa có Batch
            var pendingOrders = await _context.Orders
                .Where(o => o.Status == "New" && o.BatchId == null)
                .ToListAsync();

            if (!pendingOrders.Any()) return BadRequest("Không có đơn hàng mới nào để gom!");

            // 2. Tạo một Batch mới
            var newBatch = new PickBatch
            {
                StaffId = staffId,
                Status = "Picking",
                CreatedAt = DateTime.Now
            };

            _context.PickBatches.Add(newBatch);
            await _context.SaveChangesAsync();

            // 3. Gán mã Batch này cho các đơn hàng và đổi trạng thái đơn
            foreach (var order in pendingOrders)
            {
                order.BatchId = newBatch.BatchId;
                order.Status = "Processing";
            }

            await _context.SaveChangesAsync();
            return Ok(newBatch);
        }

        //  XUẤT DANH SÁCH TỔNG HỢP CẦN NHẶT (Sắp xếp theo Vị trí kho)
        [HttpGet("GetPickingList/{batchId}")]
        public async Task<IActionResult> GetPickingList(int batchId)
        {
            var pickingList = await _context.OrderItems
                .Where(oi => oi.Order.BatchId == batchId)
                .GroupBy(oi => new { oi.ProductId, oi.Product.Name, oi.Product.Unit })
                .Select(g => new {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    Unit = g.Key.Unit,
                    TotalToPick = g.Sum(x => x.QuantityOrdered)
                })
                .ToListAsync();

            return Ok(pickingList);
        }
    }
}
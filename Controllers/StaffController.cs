using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TramAnh_WMS.Data;
using TramAnh_WMS.Models;

namespace TramAnh_WMS.Controllers
{
 
        [Route("api/[controller]")]
        [ApiController]
        public class StaffController : ControllerBase
        {
            private readonly AppDbContext _context;

            public StaffController(AppDbContext context)
            {
                _context = context;
            }

            //  GET: api/Staff 
            [HttpGet]
            public async Task<ActionResult<IEnumerable<Staff>>> GetStaffs()
            {
                // Trả về danh sách nhân viên để bà xem ai đang quản lý kho
                return await _context.Staff.ToListAsync();
            }

            //  POST: api/Staff
            [HttpPost]
            public async Task<ActionResult<Staff>> PostStaff(Staff staff)
            {
                _context.Staff.Add(staff);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetStaffs), new { id = staff.StaffId }, staff);
            }

            // PUT: api/Staff/5 
            [HttpPut("{id}")]
            public async Task<IActionResult> PutStaff(int id, Staff staff)
            {
                if (id != staff.StaffId) return BadRequest("ID không khớp rồi bà ơi!");

                _context.Entry(staff).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }

            //  DELETE: api/Staff/5 
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteStaff(int id)
            {
                var staff = await _context.Staff.FindAsync(id);
                if (staff == null) return NotFound("Không tìm thấy nhân viên này.");

                _context.Staff.Remove(staff);
                await _context.SaveChangesAsync();

                return NoContent();
            }
        }
    
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TramAnh_WMS.Data;
using TramAnh_WMS.Models;

namespace TramAnh_WMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageLocationController : ControllerBase
    {
        private readonly AppDbContext _context;
        public StorageLocationController(AppDbContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StorageLocation>>> GetLocations()
        {
            return await _context.StorageLocations.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<StorageLocation>> PostLocation(StorageLocation location)
        {
            _context.StorageLocations.Add(location);
            await _context.SaveChangesAsync();
            return Ok(location);
        }
    }
}
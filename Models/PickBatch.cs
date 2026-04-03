using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TramAnh_WMS.Models
{
    public class PickBatch
    {
        [Key]
        public int BatchId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int StaffId { get; set; }

        public string Status { get; set; } // New, Picking, Completed

        [ForeignKey("StaffId")]
        public virtual Staff? Staff { get; set; }

        // Một Batch chứa nhiều đơn hàng (Gom đơn)
        public virtual ICollection<Order>? Orders { get; set; }
    }
}

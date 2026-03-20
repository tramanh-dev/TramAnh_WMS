using System.ComponentModel.DataAnnotations;

namespace TramAnh_WMS.Models
{
    public class Staff
    {
        [Key]
        public int StaffId { get; set; }

        [Required]
        public string FullName { get; set; }

        public string Role { get; set; } // Admin, Warehouse, StoreManager

        // Liên kết: Một nhân viên có thể phụ trách nhiều đợt gom hàng (Batch)
        public virtual ICollection<PickBatch> PickBatches { get; set; }
    }
}

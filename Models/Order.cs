using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TramAnh_WMS.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public int StoreId { get; set; }
        public int StaffId { get; set; }
        public string Status { get; set; } // New, Processing, Picked, Shipped, Cancelled

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public int? BatchId { get; set; } // Cho phép Null khi đơn hàng chưa được gom
        [ForeignKey("StaffId")]
        public virtual Staff? Staff { get; set; }

        [ForeignKey("StoreId")]
        public virtual Store? Store { get; set; }

        [ForeignKey("BatchId")]
        public virtual PickBatch? PickBatch { get; set; }

        public virtual ICollection<OrderItem>? OrderItems { get; set; }
    }
}

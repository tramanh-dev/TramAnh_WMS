using System.ComponentModel.DataAnnotations.Schema;

namespace TramAnh_WMS.Models
{
    public class OrderItem
    {
        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int QuantityOrdered { get; set; }

        public int QuantityPicked { get; set; } // Số lượng nhặt thực tế

        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}

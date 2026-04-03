using System.ComponentModel.DataAnnotations;

namespace TramAnh_WMS.Models
{
    public class Store
    {
        [Key]
        public int StoreId { get; set; }

        [Required]
        public string StoreName { get; set; }

        public string Address { get; set; }

        // Liên kết: Một cửa hàng có nhiều đơn hàng
        public virtual ICollection<Order>? Orders { get; set; }
    }
}

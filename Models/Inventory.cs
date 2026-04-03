using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TramAnh_WMS.Models
{
    public class Inventory
    {
        [Key]
        public int ProductId { get; set; } // Khóa chính 

        public int QuantityOnHand { get; set; }    // Tồn thực tế
        public int QuantityAvailable { get; set; } // Tồn có thể bán

        public int LocationId { get; set; } 

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        [ForeignKey("LocationId")]
        public virtual StorageLocation? Location { get; set; }
    }
}

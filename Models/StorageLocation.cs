using System.ComponentModel.DataAnnotations;

namespace TramAnh_WMS.Models
{
    public class StorageLocation
    {
        [Key]
        public int LocationId { get; set; }

        public string Aisle { get; set; } // Dãy
        public string Shelf { get; set; } // Kệ
        public int Level { get; set; }    // Tầng

        // Một vị trí có thể chứa thông tin tồn kho của sản phẩm
        public virtual ICollection<Inventory> Inventories { get; set; }
 
        public string LocationCode => $"{Aisle}-{Shelf}-{Level}";
    }
}

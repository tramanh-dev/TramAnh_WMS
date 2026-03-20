namespace TramAnh_WMS.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Barcode { get; set; }
        public string Unit { get; set; } // Cái, lốc, thùng
        public int CategoryId { get; set; }

        // Liên kết tới danh mục
        public virtual Category Category { get; set; }
    }
}

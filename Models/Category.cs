using System.ComponentModel.DataAnnotations;

namespace TramAnh_WMS.Models
{
    public class Category
    {
        [Key] // Thêm cái nhãn này ngay trên dòng Id
        public int CategoryId { get; set; }

        [Required]
        public string CategoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}

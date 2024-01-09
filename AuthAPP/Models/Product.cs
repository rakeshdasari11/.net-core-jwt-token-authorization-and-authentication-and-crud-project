using System.ComponentModel.DataAnnotations.Schema;

namespace AuthAPP.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public bool IsActive { get; set; }
        public Category Category { get; set; }
    }
}

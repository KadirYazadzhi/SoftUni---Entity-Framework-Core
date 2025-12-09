namespace ProductShop.Models {
    using System.ComponentModel.DataAnnotations.Schema;

    public class CategoryProduct {
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
    }
}

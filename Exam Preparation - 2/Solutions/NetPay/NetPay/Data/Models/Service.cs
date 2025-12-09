namespace NetPay.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Service
    {
        public Service()
        {
            this.Expenses = new HashSet<Expense>();
            this.SuppliersServices = new HashSet<SupplierService>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string ServiceName { get; set; } = null!;

        public virtual ICollection<Expense> Expenses { get; set; }
        public virtual ICollection<SupplierService> SuppliersServices { get; set; }
    }
}

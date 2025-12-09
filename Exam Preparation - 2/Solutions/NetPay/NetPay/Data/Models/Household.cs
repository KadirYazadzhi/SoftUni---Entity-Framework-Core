namespace NetPay.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Household
    {
        public Household()
        {
            this.Expenses = new HashSet<Expense>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string ContactPerson { get; set; } = null!;

        [MaxLength(80)]
        public string? Email { get; set; }

        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = null!;

        public virtual ICollection<Expense> Expenses { get; set; }
    }
}

namespace CarDealer.Models {
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Car {
        public Car() {
            this.PartsCars = new List<PartCar>();
            this.Sales = new List<Sale>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Make { get; set; } = null!;

        [Required]
        public string Model { get; set; } = null!;

        public long TraveledDistance { get; set; }

        public virtual ICollection<PartCar> PartsCars { get; set; }
        public virtual ICollection<Sale> Sales { get; set; }
    }
}

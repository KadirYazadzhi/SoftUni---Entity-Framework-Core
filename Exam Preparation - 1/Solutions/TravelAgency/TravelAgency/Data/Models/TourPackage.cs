namespace TravelAgency.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class TourPackage
    {
        public TourPackage()
        {
            this.Bookings = new HashSet<Booking>();
            this.TourPackagesGuides = new HashSet<TourPackageGuide>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string PackageName { get; set; } = null!;

        [MaxLength(200)]
        public string? Description { get; set; }

        public decimal Price { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<TourPackageGuide> TourPackagesGuides { get; set; }
    }
}

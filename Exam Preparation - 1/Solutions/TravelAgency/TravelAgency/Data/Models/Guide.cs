namespace TravelAgency.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using TravelAgency.Data.Models.Enums;

    public class Guide
    {
        public Guide()
        {
            this.TourPackagesGuides = new HashSet<TourPackageGuide>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(60)]
        public string FullName { get; set; } = null!;

        [Required]
        public Language Language { get; set; }

        public virtual ICollection<TourPackageGuide> TourPackagesGuides { get; set; }
    }
}

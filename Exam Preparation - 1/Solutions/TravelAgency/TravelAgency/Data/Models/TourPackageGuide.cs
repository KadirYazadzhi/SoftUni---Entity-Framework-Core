namespace TravelAgency.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class TourPackageGuide
    {
        public int TourPackageId { get; set; }
        [ForeignKey(nameof(TourPackageId))]
        public virtual TourPackage TourPackage { get; set; } = null!;

        public int GuideId { get; set; }
        [ForeignKey(nameof(GuideId))]
        public virtual Guide Guide { get; set; } = null!;
    }
}

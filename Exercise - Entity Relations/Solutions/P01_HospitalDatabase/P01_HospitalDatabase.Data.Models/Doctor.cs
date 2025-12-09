namespace P01_HospitalDatabase.Data.Models {
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Doctor {
        public Doctor() {
            this.Visitations = new List<Visitation>();
        }

        [Key]
        public int DoctorId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Specialty { get; set; } = null!;

        public virtual ICollection<Visitation> Visitations { get; set; }
    }
}

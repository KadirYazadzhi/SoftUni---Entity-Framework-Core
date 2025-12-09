namespace P01_HospitalDatabase.Data.Models {
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Patient {
        public Patient() {
            this.Visitations = new List<Visitation>();
            this.Diagnoses = new List<Diagnose>();
            this.Prescriptions = new List<PatientMedicament>();
        }

        [Key]
        public int PatientId { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = null!;

        [Required]
        [MaxLength(250)]
        public string Address { get; set; } = null!;

        [MaxLength(80)]
        public string? Email { get; set; }

        public bool HasInsurance { get; set; }

        public virtual ICollection<Visitation> Visitations { get; set; }
        public virtual ICollection<Diagnose> Diagnoses { get; set; }
        public virtual ICollection<PatientMedicament> Prescriptions { get; set; }
    }
}

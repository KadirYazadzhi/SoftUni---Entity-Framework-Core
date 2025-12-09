namespace AcademicRecordsApp.Data.Models {
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Exam {
        public Exam() {
            this.Grades = new HashSet<Grade>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public int? CourseId { get; set; }
        public virtual Course? Course { get; set; }

        public virtual ICollection<Grade> Grades { get; set; }
    }
}

namespace AcademicRecordsApp.Data.Models {
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Course {
        public Course() {
            this.Exams = new HashSet<Exam>();
            this.Students = new HashSet<Student>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public virtual ICollection<Exam> Exams { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}

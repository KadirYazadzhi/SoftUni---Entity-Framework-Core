namespace AcademicRecordsApp.Data.Models {
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Student {
        public Student() {
            this.Grades = new HashSet<Grade>();
            this.Courses = new HashSet<Course>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;

        public virtual ICollection<Grade> Grades { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
    }
}

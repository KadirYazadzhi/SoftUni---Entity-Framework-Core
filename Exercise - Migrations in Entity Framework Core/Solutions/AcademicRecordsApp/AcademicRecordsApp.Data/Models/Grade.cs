namespace AcademicRecordsApp.Data.Models {
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Grade {
        [Key]
        public int Id { get; set; }

        public decimal Value { get; set; }

        public int ExamId { get; set; }
        [ForeignKey(nameof(ExamId))]
        public virtual Exam Exam { get; set; } = null!;

        public int StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; } = null!;
    }
}

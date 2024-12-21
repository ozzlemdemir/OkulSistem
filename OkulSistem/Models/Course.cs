namespace OkulSistem.Models
{
    public class Course
    {
        public string CourseID { get; set; } // Ders ID'si (primary key)
        public string? CourseName { get; set; } // Ders Adı
        public string? Credits { get; set; } // Kredi
        public string? InstructorID {  get; set; }

        public Instructor? Instructor { get; set; }
       

        public ICollection<StudentsCourse> StudentCourses { get; set; } = new List<StudentsCourse>();
    }
}

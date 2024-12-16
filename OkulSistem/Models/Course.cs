namespace OkulSistem.Models
{
    public class Course
    {
        public string CourseID { get; set; } // Ders ID'si (primary key)
        public string? CourseName { get; set; } // Ders Adı
        public string? Credits { get; set; } // Kredi
        public string? InsturctorID { get; set; } // Bölüm hocası

        public ICollection<StudentsCourse> StudentCourses { get; set; } = new List<StudentsCourse>();
    }
}

namespace OkulSistem.Models
{
    public class Student
    {
        public string StudentID { get; set; } // Öğrenci ID'si (primary key)
        public string? FirstName { get; set; } // Öğrenci Adı
        public string? LastName { get; set; } // Öğrenci Soyadı
        public string? Email { get; set; } // Öğrenci E-posta Adresi
        public string? Department { get; set; }//öğrenci departmanı
        public string? Password { get; set; } //öğrenci şifresi
        public string? Role {  get; set; }

        public ICollection<StudentsCourse> StudentCourses { get; set; } = new List<StudentsCourse>();
        public ICollection<InstructorCourse> InstructorCourses { get; set; }
    }
}

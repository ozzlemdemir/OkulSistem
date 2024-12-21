namespace OkulSistem.Models
{
    public class StudentsCourse
    {
        public int SelectionID { get; set; }//ders seçimi piramary key
        public string? StudentID { get; set; } // Öğrenci ID'si
        public string? CourseID { get; set; } // Ders ID'si
        public string? StudentName { get; set; }
        public string? StudentLastName { get; set; }

        public Student? Student { get; set; } // Navigation property (ilişkili öğrenci)
        public Course? Course { get; set; } // Navigation property (ilişkili ders)
    }
}

namespace OkulSistem.Models
{
    public class InstructorCourse
    {
        public int InstructorCourseID {  get; set; }
        public string? StudentID { get; set; } 
        public string? CourseID { get; set; }
        public string? InstructorID{ get; set; }
        public bool? Onay {  get; set; }

      
        public Course? Course { get; set; } // Navigation property (ilişkili ders)
        public Instructor? Instructor { get; set; }// Navigation property(ilişkili instructor)
    }
}

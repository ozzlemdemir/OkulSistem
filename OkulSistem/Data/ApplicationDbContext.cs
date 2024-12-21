using Microsoft.EntityFrameworkCore;
using OkulSistem.Models;

namespace OkulSistem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentsCourse> StudentsCourses { get; set; }
        public DbSet<InstructorCourse> InstructorCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
            modelBuilder.Entity<StudentsCourse>()
                .HasKey(sc => new { sc.SelectionID });

            modelBuilder.Entity<StudentsCourse>()
               .Property(sc => sc.SelectionID)
                       .ValueGeneratedOnAdd();


            modelBuilder.Entity<InstructorCourse>()
         .HasOne(ic => ic.Student)
         .WithMany(s => s.InstructorCourses) 
         .HasForeignKey(ic => ic.StudentID)
         .OnDelete(DeleteBehavior.Cascade);
            base.OnModelCreating(modelBuilder);
        }




    }




}

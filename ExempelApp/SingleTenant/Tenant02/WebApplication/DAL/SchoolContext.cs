using WebApplication.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Web;
using System.Diagnostics;

namespace WebApplication.DAL
{
    public class SchoolContext : DbContext
    {
        public SchoolContext()
            : this("Server=tcp:exjobb-exempelapp.database.windows.net,1433;Initial Catalog=ExjobbSingle02;Persist Security Info=False;User ID=Guest_CRM;Password=TreasuryGast!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;")
        {}

        public SchoolContext(string connectionString)
            : base("Server=tcp:exjobb-exempelapp.database.windows.net,1433;Initial Catalog=ExjobbSingle02;Persist Security Info=False;User ID=Guest_CRM;Password=TreasuryGast!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;")
        {}

        public DbSet<Course> Courses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }
        public DbSet<Person> People { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Instructors).WithMany(i => i.Courses)
                .Map(t => t.MapLeftKey("CourseID")
                    .MapRightKey("InstructorID")
                    .ToTable("CourseInstructor"));

            modelBuilder.Entity<Department>().MapToStoredProcedures();
        }
    }
}
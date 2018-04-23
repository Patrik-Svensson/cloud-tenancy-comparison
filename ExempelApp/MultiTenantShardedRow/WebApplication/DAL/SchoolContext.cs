using WebApplication.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Web;
using System.Diagnostics;
using System;

namespace WebApplication.DAL
{
    public class SchoolContext : DbContext
    {
        [Obsolete("Do not call this, it is used for Add-Migration support")]
        public SchoolContext()
            : this(GetConnectionString())
        { }

        public SchoolContext(string connectionString)
            : base(connectionString)
        {

        }
        
        public SchoolContext(ISettingsProvider settingsProvider)
            : this((settingsProvider.GetConnectionString()))
        { }

        private static string GetConnectionString()
        {
            return "Server=tcp:exjobb-exempelapp.database.windows.net,1433;Initial Catalog=ExjobbSharded02;Persist Security Info=False;User ID=Guest_CRM;Password=TreasuryGast!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        }

        /*private static string GetConnectionString(ITenantIdProvider idProvider)
        {
            //return CatalogSettingsProvider.GetConnectionString();
            //return (string)ConnectionStringProvider.connectionstringProvider.Get(idProvider);
            return Common.ConnectionTenantDb.GetConnectionStringForTenant(idProvider.TenantId());
        }*/
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
using Academy.Core.Models;
using Academy.Core.Models.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Repo.Data
{
    public class AcademyContext : DbContext
    {
        public AcademyContext(DbContextOptions<AcademyContext> options) : base(options) 
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
			
			base.OnModelCreating(modelBuilder);

        }
		

		public DbSet<Student> Students { get; set; }
        public DbSet<Coordinator> Coordinates { get; set; }
        public DbSet<StudentAffair> StudentAffairs { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<AvailableCourse> Availablecourses { get; set; }
        public DbSet<AssignedCourse> Assignedcourses { get; set; }

        public DbSet<FinalExamTimeTable> FinalExamTimeTable { get; set; }
        public DbSet<ScheduleTimeTable> ScheduleTimeTable { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Report> Reports { get; set; }

    }
}

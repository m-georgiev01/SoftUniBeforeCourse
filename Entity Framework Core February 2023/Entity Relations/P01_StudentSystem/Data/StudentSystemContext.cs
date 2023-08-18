using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;
using P01_StudentSystem.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace P01_StudentSystem.Data
{
    public class StudentSystemContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Homework> Homeworks { get; set; }
       // public DbSet<StudentCourse> StudentCourse { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=ExDemo;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //convert Enum int value to string value
            modelBuilder.Entity<Homework>()
                .Property(h => h.ContentType)
                .HasConversion(
                    value => value.ToString(),
                    value => (ContentTypes)Enum.Parse(typeof(ContentTypes), value));

            modelBuilder.Entity<Resource>()
                .Property(r => r.ResourceType)
                .HasConversion(
                    value => value.ToString(),
                    value => (ResourceTypes)Enum.Parse(typeof(ResourceTypes), value));

            //Many-to-Many
            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.StudentId, sc.CourseId });

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentId);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId);
        }
    }
}

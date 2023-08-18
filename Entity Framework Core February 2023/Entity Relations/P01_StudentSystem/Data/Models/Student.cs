using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Data.Models
{
    public class Student
    {
        public Student()
        {
            Homeworks = new HashSet<Homework>();
            StudentCourses = new HashSet<StudentCourse>();
        }

        public int StudentId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [Column(TypeName = "CHAR(10)")]
        [Unicode(false)]
        public string? PhoneNumber { get; set; }
        public DateTime RegisteredOn { get; set; }
        public DateTime? Birthday { get; set; }

        public ICollection<Homework> Homeworks { get; set; }
        public ICollection<StudentCourse> StudentCourses { get; set; }
    }
}

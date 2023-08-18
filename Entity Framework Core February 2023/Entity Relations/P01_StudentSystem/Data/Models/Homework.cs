using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Enums;

namespace P01_StudentSystem.Data.Models
{
    public class Homework
    {
        public int HomeworkId { get; set; }

        [Unicode(false)]
        public string Content { get; set; }
        public ContentTypes ContentType { get; set; }
        public DateTime SubmissionTime { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Enums;
using System.ComponentModel.DataAnnotations;


namespace P01_StudentSystem.Data.Models
{
    public class Resource
    {
        public int ResourceId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [Unicode(false)]
        public string Url { get; set; }

        public ResourceTypes ResourceType { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}

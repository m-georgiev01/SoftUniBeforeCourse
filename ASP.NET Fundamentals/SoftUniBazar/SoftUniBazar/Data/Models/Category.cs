using System.ComponentModel.DataAnnotations;
using static SoftUniBazar.Data.DataConstants;

namespace SoftUniBazar.Data.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(CategoryNameMax)]
        public string Name { get; set; } = null!;

        public IEnumerable<Ad> Ads { get; set; } = new List<Ad>();
    }
}

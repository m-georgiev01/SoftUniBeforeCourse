using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SoftUniBazar.Data.DataConstants;

namespace SoftUniBazar.Data.Models
{
    public class Ad
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(AdNameMax)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(AdDescriptionMax)]
        public string Description { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string ImageUrl { get; set; } = null!;

        [Required]
        public DateTime CreatedOn { get; set; }


        [Required]
        public string OwnerId { get; set; } = null!;

        [Required]
        public IdentityUser Owner { get; set; } = null!;

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;
    }
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Ex_api_DTO.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Category { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public double Price { get; set; }
    }
}

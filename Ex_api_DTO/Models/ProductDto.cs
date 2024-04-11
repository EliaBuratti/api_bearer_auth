using System.ComponentModel.DataAnnotations;

namespace Ex_api_DTO.Models
{
    public class ProductDto
    {

        public string Name { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }
    }
}

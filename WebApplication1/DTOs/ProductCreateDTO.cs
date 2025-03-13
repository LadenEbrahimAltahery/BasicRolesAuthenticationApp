using System.ComponentModel.DataAnnotations;

namespace RoleBasedBasicAuthentication.DTOs
{
    public class ProductCreateDTO
    {
        [Required]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Required]
        [Range(0.0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 1000, ErrorMessage = "Quantiy must be a positive value between 0 and 1000.")]
        public int Quantity { get; set; }
    }
}
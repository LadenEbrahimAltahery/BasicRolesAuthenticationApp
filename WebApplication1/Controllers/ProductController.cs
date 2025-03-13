using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoleBasedBasicAuthentication.Models;
using RoleBasedBasicAuthentication.DTOs;
using RoleBasedBasicAuthenticationDemo.Services;
using RoleBasedBasicAuthentication.DTOs;
using RoleBasedBasicAuthentication.Interfaces;
using RoleBasedBasicAuthentication.Models;

namespace RoleBasedBasicAuthenticationDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "BasicAuthentication")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/Products/GetAllProductsAsync
        [HttpGet("GetAllProductsAsync")]
        [Authorize(Policy = "AdminOrUser")] // Accessible by Admin or User
        public async Task<ActionResult<IEnumerable<ProductReadDTO>>> GetAllProductsAsync()
        {
            var products = await _productService.GetAllProductsAsync();

            var productDtos = products.Select(p => new ProductReadDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Quantity = p.Quantity,
            }).ToList();

            return Ok(productDtos);
        }

        // GET: api/Products/GetProductByIdAsync/1
        [HttpGet("GetProductByIdAsync/{id}")]
        [Authorize(Policy = "UserOnly")] // Accessible by User only
        public async Task<ActionResult<ProductReadDTO>> GetProductByIdAsync(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            var productDto = new ProductReadDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
            };

            return Ok(productDto);
        }

        // POST: api/Products/CreateProductAsync
        [HttpPost("CreateProductAsync")]
        [Authorize(Policy = "AdminOnly")] // Only Admin can create products
        public async Task<ActionResult<ProductReadDTO>> CreateProductAsync([FromBody] ProductCreateDTO productCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Map DTO to Entity
            var product = new Product
            {
                Name = productCreateDto.Name,
                Description = productCreateDto.Description,
                Price = productCreateDto.Price,
                Quantity = productCreateDto.Quantity,
            };

            product = await _productService.CreateProductAsync(product);

            // Map Entity to Read DTO
            var productReadDto = new ProductReadDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
            };

            return Ok(productReadDto);
        }

        // PUT: api/Products/UpdateProductAsync/1
        [HttpPut("UpdateProductAsync/{id}")]
        [Authorize(Policy = "AdminOrUser")] // Accessible by Admin or User
        public async Task<IActionResult> UpdateProductAsync(int id, [FromBody] ProductUpdateDTO productUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != productUpdateDto.Id)
                return BadRequest("Product ID mismatch.");

            // Map DTO to Entity
            var product = new Product
            {
                Id = productUpdateDto.Id,
                Name = productUpdateDto.Name,
                Description = productUpdateDto.Description,
                Price = productUpdateDto.Price,
                Quantity = productUpdateDto.Quantity,
            };

            var updated = await _productService.UpdateProductAsync(product);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/Products/DeleteProductAsync/1
        [HttpDelete("DeleteProductAsync/{id}")]
        [Authorize(Policy = "AdminAndUser")] // Accessible by user having both Admin and User Role
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            var deleted = await _productService.DeleteProductAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
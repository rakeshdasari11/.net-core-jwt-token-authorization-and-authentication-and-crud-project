using AuthAPP.Data;
using AuthAPP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthAPP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public ProductsController(ApiDbContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("GetProductById")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        [HttpPut("EditProduct")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> EditProduct(Product product)
        {
            //if (id != product.ProductId)
            //{
            //    return BadRequest();
            //}

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.ProductId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        [HttpPost("AddProduct")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult<Product>> AddProduct(Product product)
        {
            if (product.Category.CategoryId != 0) // Assuming 0 represents no category
            {
                // Check if the specified category exists
                var existingCategory = await _context.Categories.FindAsync(product.Category.CategoryId);
                if (existingCategory == null)
                {
                    // Return a 404 Not Found response if the category doesn't exist
                    return NotFound("The specified category does not exist.");
                }

                // Associate the product with the existing category
                product.Category = existingCategory;
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("DeleteProduct")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}

using AuthAPP.Data;
using AuthAPP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrudAPI.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public CategoriesController(ApiDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        // GET: api/Categories/5
        [HttpGet("GetCategoryById")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult<Category>> GetCategoryById(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/Categories/5
        [HttpPut("EditCategory")]
       // [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> EditCategory(Category category)
        {
            //if (category.CategoryId == )
            //{
            //    return BadRequest();
            //}

            var existingCategory = await _context.Categories.FindAsync(category.CategoryId);
            if (existingCategory == null)
            {
                return NotFound();
            }

            existingCategory.Name = category.Name;
            existingCategory.IsActive = category.IsActive;

            // Deactivate or activate related products
            if (!category.IsActive)
            {
                var relatedProducts = await _context.Products.Where(p => p.Category.CategoryId == category.CategoryId).ToListAsync();
                foreach (var product in relatedProducts)
                {
                    product.IsActive = false;
                }
            }
            else // Activate related products when the category is activated
            {
                var relatedProducts = await _context.Products.Where(p => p.Category.CategoryId == category.CategoryId).ToListAsync();
                foreach (var product in relatedProducts)
                {
                    product.IsActive = true;
                }
            }

            try
            {
               
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.CategoryId))
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

        // POST: api/Categories
        [HttpPost("AddCategory")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult<Category>> AddCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.CategoryId }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("DeleteCategory")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
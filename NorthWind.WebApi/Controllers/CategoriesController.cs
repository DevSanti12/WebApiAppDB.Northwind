using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthWind.WebApi.Models;

namespace NorthWind.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly NorthwindContext _context;

        public CategoriesController(NorthwindContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categories>>> GetCategories()
        {
            try
            {
                return await _context.Categories.ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving categories from database");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetCategory(int id)
        {
            try
            {
                if (id < 0)
                {
                    return BadRequest("Category id cannot be negative.");
                }

                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound();
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving category from database");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Categories>> PostCategory(Categories category)
        {
            try
            {
                if (category == null)
                {
                    return BadRequest("Category cannot be null.");
                }

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    "GetCategory",
                    new { id = category.CategoryID },
                    category);
            }
            catch (DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating category in database");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error processing category creation");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Categories>> PutCategory(int id, Categories Category)
        {
            try
            {
                if (id != Category.CategoryID)
                    return BadRequest();

                var existingCategory = await _context.Categories.FindAsync(id);

                if (existingCategory == null)
                    return NotFound();

                // Update only the properties that are specified in the request
                _context.Entry(existingCategory).CurrentValues.SetValues(Category);

                // Ensure that only modified properties are updated
                foreach (var property in _context.Entry(Category).Properties)
                {
                    if (property.CurrentValue == null)
                    {
                        _context.Entry(existingCategory).Property(property.Metadata.Name).IsModified = false;
                    }
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Categories.Any(p => p.CategoryID == id))
                        return NotFound();
                    throw;
                }

                return Ok(existingCategory);
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(StatusCodes.Status409Conflict,
                    "Category was modified by another user");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating category");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Categories>> DeleteCategory(int id)
        {
            try
            {
                var Category = await _context.Categories.FindAsync(id);
                if (Category == null)
                {
                    return NotFound();
                }

                _context.Categories.Remove(Category);
                await _context.SaveChangesAsync();

                return Category;
            }
            catch (DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting category. It might be referenced by other records.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error processing category deletion");
            }
        }
    }
}

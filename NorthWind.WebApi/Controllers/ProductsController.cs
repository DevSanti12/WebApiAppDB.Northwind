using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthWind.WebApi.Models;

namespace NorthWind.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly NorthwindContext _context;

        public ProductsController(NorthwindContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            if (id < 0)
            {
                return BadRequest("Product id cannot be negative.");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Products>> PostProduct(Products product)
        {
            if(product == null)
            {
                return BadRequest("Product cannot be null.");
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                "GetProduct",
                new { id = product.ProductID },
                product);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Products>> PutProduct(int id, Products product)
        {
            if (id != product.ProductID)
                return BadRequest();

            var existingProduct = await _context.Products.FindAsync(id);

            if (existingProduct == null)
                return NotFound();

            // Update only the properties that are specified in the request
            _context.Entry(existingProduct).CurrentValues.SetValues(product);

            // Ensure that only modified properties are updated
            foreach (var property in _context.Entry(product).Properties)
            {
                if (property.CurrentValue == null)
                {
                    _context.Entry(existingProduct).Property(property.Metadata.Name).IsModified = false;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(p => p.ProductID == id))
                    return NotFound();
                else
                    throw;
            }

            return Ok(existingProduct);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Products>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return product;
        }


        // Added pagination and filtering for list operation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Products>>> GetProducts(
            int pageNumber = 1, int pageSize = 10, int? categoryId = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _context.Products.AsQueryable();
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryID == categoryId);
            }

            var paginatedResults = await query
                .OrderBy(p => p.ProductID) // Order by a consistent key (e.g., ProductID)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            int totalCount = await query.CountAsync();

            var response = new
            {
                pagination = new
                {
                    currentPage = pageNumber,
                    pageSize = pageSize,
                    totalCount = totalCount,
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                },
                data = paginatedResults
            };

            return Ok(response);
        }
    }
}

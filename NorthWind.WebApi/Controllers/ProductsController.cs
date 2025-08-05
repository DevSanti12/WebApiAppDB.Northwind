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
            try
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
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving product from database");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Products>> PostProduct(Products product)
        {
            try
            {
                if (product == null)
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
            catch (DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating product in database");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error processing product creation");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Products>> PutProduct(int id, Products product)
        {
            try
            {
                if (id != product.ProductID)
                    return BadRequest();

                var existingProduct = await _context.Products.FindAsync(id);

                if (existingProduct == null)
                    return NotFound();

                _context.Entry(existingProduct).CurrentValues.SetValues(product);

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
                    throw;
                }

                return Ok(existingProduct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(StatusCodes.Status409Conflict, "Product was modified by another user");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating product");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Products>> DeleteProduct(int id)
        {
            try
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
            catch (DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting product. It might be referenced by other records.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error processing product deletion");
            }
        }


        // Added pagination and filtering for list operation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Products>>> GetProducts(
            int pageNumber = 1, int pageSize = 10, int? categoryId = null)
        {
            try
            {
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _context.Products.AsQueryable();
                if (categoryId.HasValue)
                {
                    query = query.Where(p => p.CategoryID == categoryId);
                }

                int totalCount = await query.CountAsync();
                int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var paginatedResults = await query
                    .OrderBy(p => p.ProductID)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Build base URL
                var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

                // Helper to build query string
                string BuildUrl(int targetPage)
                {
                    var queryParams = new List<string>
                    {
                        $"pageNumber={targetPage}",
                        $"pageSize={pageSize}"
                    };
                    if (categoryId.HasValue)
                        queryParams.Add($"categoryId={categoryId.Value}");
                    return $"{baseUrl}?{string.Join("&", queryParams)}";
                }

                // Build Link header
                var links = new List<string>();
                if (pageNumber > 1)
                    links.Add($"<{BuildUrl(1)}>; rel=\"first\"");
                if (pageNumber > 1)
                    links.Add($"<{BuildUrl(pageNumber - 1)}>; rel=\"prev\"");
                if (pageNumber < totalPages)
                    links.Add($"<{BuildUrl(pageNumber + 1)}>; rel=\"next\"");
                if (pageNumber < totalPages)
                    links.Add($"<{BuildUrl(totalPages)}>; rel=\"last\"");

                if (links.Count > 0)
                    Response.Headers["Link"] = string.Join(", ", links);

                // Add nextPageUrl to response body
                string? nextPageUrl = pageNumber < totalPages ? BuildUrl(pageNumber + 1) : null;

                var response = new
                {
                    pagination = new
                    {
                        currentPage = pageNumber,
                        pageSize = pageSize,
                        totalCount = totalCount,
                        totalPages = totalPages,
                        nextPageUrl = nextPageUrl
                    },
                    data = paginatedResults
                };

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving products from database");
            }
        }
    }
}

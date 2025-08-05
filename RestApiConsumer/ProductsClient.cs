using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace RestApiConsumer;

public class ProductsClient
{
    private readonly HttpClient _httpClient;

    public ProductsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Fetch a list of products with optional pagination and filtering
    public async Task<PaginatedResponse<Product>> GetProductsAsync(int pageNumber = 1, int pageSize = 10, int? categoryId = null)
    {
        string endpoint = $"api/Products?pageNumber={pageNumber}&pageSize={pageSize}";
        if (categoryId.HasValue)
        {
            endpoint += $"&categoryId={categoryId}";
        }

        var response = await _httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();

        var paginatedResponse = await response.Content.ReadFromJsonAsync<PaginatedResponse<Product>>();

        return paginatedResponse; // Return both metadata and data
    }

    // Fetch a single product by ID
    public async Task<Product> GetProductByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/Products/{id}");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Product>();
    }

    // Create a new product
    public async Task<Product> CreateProductAsync(Product newProduct)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/Products", newProduct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Product>();
    }

    // Update an existing product
    public async Task UpdateProductAsync(int id, Product updatedProduct)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/Products/{id}", updatedProduct);
        response.EnsureSuccessStatusCode();
    }

    // Delete a product
    public async Task DeleteProductAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/Products/{id}");
        response.EnsureSuccessStatusCode();
    }
}

// Define the Product model to match the API's Product resource

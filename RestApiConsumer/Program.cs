using System;
using System.Net.Http;
using System.Threading.Tasks;
using RestApiConsumer;

class Program
{
    static async Task Main(string[] args)
    {
        // Initialize HttpClient with the base address of your API
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7097/") // Replace with your API's base URL
        };

        var client = new ProductsClient(httpClient);

        try
        {
            Console.WriteLine("Fetching products (page 1)...");
            var products = await client.GetProductsAsync();
            foreach (var product in products.Data)
            {
                Console.WriteLine($"ID: {product.ProductID}, Name: {product.ProductName}");
            }

            Console.WriteLine("\nCreating a new product...");
            var newProduct = new Product
            {
                ProductName = "New Product",
                CategoryID = 1,
                SupplierID = 2,
                QuantityPerUnit = "10 boxes",
                UnitPrice = 50.75M,
                UnitsInStock = 100,
                Discontinued = false
            };

            var createdProduct = await client.CreateProductAsync(newProduct);
            Console.WriteLine($"Created product with ID: {createdProduct.ProductID}");

            Console.WriteLine("\nUpdating the created product...");
            createdProduct.ProductName = "Updated Product";
            await client.UpdateProductAsync(createdProduct.ProductID, createdProduct);
            Console.WriteLine("Product updated successfully.");

            Console.WriteLine("\nFetching updated product by ID...");
            var updatedProduct = await client.GetProductByIdAsync(createdProduct.ProductID);
            Console.WriteLine($"Updated Product - ID: {updatedProduct.ProductID}, Name: {updatedProduct.ProductName}");

            Console.WriteLine("\nDeleting the created product...");
            await client.DeleteProductAsync(createdProduct.ProductID);
            Console.WriteLine("Product deleted successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}

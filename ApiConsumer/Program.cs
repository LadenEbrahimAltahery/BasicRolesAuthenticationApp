using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BasicAuthConsoleClient
{
    public class ProductCreateDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class ProductReadDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class ProductUpdateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class Program
    {
        // Change baseUrl to match your ASP.NET Core Web API address/port.
        private static readonly string baseUrl = "https://localhost:7051"; // Update PORT as necessary

        private static async Task Main(string[] args)
        {
            Console.WriteLine("Basic Auth Console Client with Role-Based Authorization");

            //"john.doe@example.com", "password123", "Admin"
            //"jane.smith@example.com", "password123", "User"
            //"hina.sharma@example.com", "password123", "Admin and User"
            //"sara.taylor@example.com", "password123", "No Roles"
            var username = "hina.sharma@example.com";
            var password = "password123";

            // Create and configure HttpClient
            using var httpClient = new HttpClient();

            // Attach Basic Authentication header
            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            Console.WriteLine($"\nAuthenticated as: {username} ");

            // 1. GET all products
            await GetAllProductsAsync(httpClient);

            // 2. GET a single product by Id (e.g., Id = 1)
            await GetProductByIdAsync(httpClient, 1);

            // 3. CREATE (POST) a new product
            var newProductId = await CreateProductAsync(httpClient);

            // 4. UPDATE (PUT) the new product
            if (newProductId > 0)
                await UpdateProductAsync(httpClient, newProductId);

            // 5. DELETE the product
            if (newProductId > 0)
                await DeleteProductAsync(httpClient, newProductId);

            Console.WriteLine("\nAll Operations are Completed");
            Console.ReadKey();
        }

        private static async Task GetAllProductsAsync(HttpClient httpClient)
        {
            Console.WriteLine("\n--- GET ALL PRODUCTS ---");
            try
            {
                var response = await httpClient.GetAsync($"{baseUrl}/api/Products/GetAllProductsAsync");
                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    Console.WriteLine("Access Forbidden: You do not have permission to view products.");
                    return;
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("Unauthorized: Invalid Credenials");
                    return;
                }

                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<List<ProductReadDTO>>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                Console.WriteLine("Products:");
                if (products != null)
                {
                    foreach (var product in products)
                    {
                        Console.WriteLine($"Id: {product.Id}, Name: {product.Name}, Price: {product.Price}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching products: {ex.Message}");
            }
        }

        private static async Task GetProductByIdAsync(HttpClient httpClient, int id)
        {
            Console.WriteLine($"\n--- GET PRODUCT BY ID: {id} ---");
            try
            {
                var response = await httpClient.GetAsync($"{baseUrl}/api/Products/GetProductByIdAsync/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    Console.WriteLine("Access Forbidden: You do not have permission to view this product.");
                    return;
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("Unauthorized: Invalid Credenials");
                    return;
                }

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to get product. HTTP Status: {response.StatusCode}");
                    return;
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<ProductReadDTO>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (product != null)
                {
                    Console.WriteLine($"Product Found - Id: {product.Id}, Name: {product.Name}, Price: {product.Price}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching product by Id: {ex.Message}");
            }
        }

        private static async Task<int> CreateProductAsync(HttpClient httpClient)
        {
            Console.WriteLine("\n--- CREATE NEW PRODUCT (POST) ---");
            var newProduct = new ProductCreateDTO
            {
                Name = $"Product_{Guid.NewGuid()}",
                Description = "A new test product.",
                Price = 99.99M,
                Quantity = 100
            };

            try
            {
                var payload = JsonSerializer.Serialize(newProduct);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{baseUrl}/api/Products/CreateProductAsync", content);
                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    Console.WriteLine("Access Forbidden: You do not have permission to create products.");
                    return 0;
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("Unauthorized: Invalid Credenials");
                    return 0;
                }

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to create product. HTTP Status: {response.StatusCode}");
                    return 0;
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var createdProduct = JsonSerializer.Deserialize<ProductReadDTO>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (createdProduct != null)
                {
                    Console.WriteLine($"Created product Id: {createdProduct.Id}, Name: {createdProduct.Name}");
                    return createdProduct.Id;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating product: {ex.Message}");
            }

            return 0;
        }

        private static async Task UpdateProductAsync(HttpClient httpClient, int productId)
        {
            Console.WriteLine("\n--- UPDATE PRODUCT (PUT) ---");
            var updatedProduct = new ProductUpdateDTO
            {
                Id = productId,
                Name = $"Updated_Product_{Guid.NewGuid()}",
                Description = "An updated test product.",
                Price = 149.99M,
                Quantity = 10
            };

            try
            {
                var payload = JsonSerializer.Serialize(updatedProduct);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync($"{baseUrl}/api/Products/UpdateProductAsync/{productId}", content);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("Unauthorized: Invalid Credenials");
                    return;
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    Console.WriteLine("Access Forbidden: You do not have permission to update products.");
                    return;
                }

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to update product. HTTP Status: {response.StatusCode}");
                    return;
                }

                Console.WriteLine("Product successfully updated.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating product: {ex.Message}");
            }
        }

        private static async Task DeleteProductAsync(HttpClient httpClient, int productId)
        {
            Console.WriteLine("\n--- DELETE PRODUCT ---");
            try
            {
                var response = await httpClient.DeleteAsync($"{baseUrl}/api/Products/DeleteProductAsync/{productId}");
                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    Console.WriteLine("Access Forbidden: You do not have permission to delete products.");
                    return;
                }
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("Unauthorized: Invalid Credenials");
                    return;
                }

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to delete product. HTTP Status: {response.StatusCode}");
                    return;
                }

                Console.WriteLine($"Product with Id: {productId} deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting product: {ex.Message}");
            }
        }
    }
}
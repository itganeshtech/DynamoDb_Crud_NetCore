using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.DependencyInjection;
using ProductDIExample.Models;
using ProductDIExample.Services;

namespace ProductDIExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var config = new AmazonDynamoDBConfig
            {
                ServiceURL = "http://localhost:8001",
                UseHttp = true
            };

            // Create DynamoDB client & context
           
            var client = new AmazonDynamoDBClient(config);
            var context = new DynamoDBContext(client);

            // Register with Dependency Injection
            services.AddSingleton<IAmazonDynamoDB>(client);
            services.AddSingleton<IDynamoDBContext>(context);
            services.AddScoped<IDataService, DynamoDbDataService>();
            // Add controllers or minimal APIs
            services.AddControllers();
            var app = builder.Build();
            // Ensure DynamoDB table exists
            await EnsureTableExistsAndSeedAsync(client, context, "Products");


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                //pattern: "{controller=Home}/{action=Index}/{id?}");
                pattern: "{controller=Product}/{action=Index}/{id?}");

            app.Run();
        }

        // Creates table if it doesn't exist, and seeds data
        static async Task EnsureTableExistsAndSeedAsync(IAmazonDynamoDB client, IDynamoDBContext context, string tableName)
        {
            var tables = await client.ListTablesAsync();

            if (!tables.TableNames.Contains(tableName))
            {
                Console.WriteLine($"Creating DynamoDB table '{tableName}'...");

                var request = new CreateTableRequest
                {
                    TableName = tableName,
                    AttributeDefinitions = new List<AttributeDefinition>
            {
                new AttributeDefinition
                {
                    AttributeName = "Id",
                    AttributeType = "S"
                }
            },
                    KeySchema = new List<KeySchemaElement>
            {
                new KeySchemaElement
                {
                    AttributeName = "Id",
                    KeyType = "HASH"
                }
            },
                    BillingMode = BillingMode.PAY_PER_REQUEST
                };

                await client.CreateTableAsync(request);
                await WaitForTableToBecomeActiveAsync(client, tableName);

                Console.WriteLine($"Table '{tableName}' created successfully!");

                // Seed sample data after table is ready
                await SeedSampleDataAsync(context);
            }
            else
            {
                Console.WriteLine($"Table '{tableName}' already exists. Skipping creation.");
            }
        }

        // Wait until table becomes active
        static async Task WaitForTableToBecomeActiveAsync(IAmazonDynamoDB client, string tableName)
        {
            string status = null;
            do
            {
                await Task.Delay(2000);
                var response = await client.DescribeTableAsync(tableName);
                status = response.Table.TableStatus;
                Console.WriteLine($"Waiting for table '{tableName}' to become ACTIVE (current: {status})...");
            }
            while (status != "ACTIVE");
        }

        // Seed initial products
        static async Task SeedSampleDataAsync(IDynamoDBContext context)
        {
            var sampleProducts = new List<Product>
    {
        new Product { Id = "101", Name = "Laptop", Price = 49999 },
        new Product { Id = "102", Name = "Smartphone", Price = 29999 },
        new Product { Id = "103", Name = "Wireless Mouse", Price = 999 }
    };

            Console.WriteLine("Seeding sample data into 'Products' table...");

            foreach (var product in sampleProducts)
            {
                await context.SaveAsync(product);
                Console.WriteLine($"Inserted sample item: {product.Id} - {product.Name}");
            }

            Console.WriteLine("Sample data seeding completed!");
        }

    }
}

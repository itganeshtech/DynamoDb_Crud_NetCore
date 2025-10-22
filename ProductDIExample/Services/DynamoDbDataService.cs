using Amazon.DynamoDBv2.DataModel;
using ProductDIExample.Models;

namespace ProductDIExample.Services
{
    public class DynamoDbDataService : IDataService
    {
        private readonly IDynamoDBContext _context;

        public DynamoDbDataService(IDynamoDBContext context)
        {
            _context = context;
        }

        public async Task SaveProductAsync(Product product)
        {
            await _context.SaveAsync(product);
        }

        public async Task<Product> GetProductAsync(string id)
        {
            return await _context.LoadAsync<Product>(id);
        }

        public async Task DeleteProductAsync(string id)
        {
            await _context.DeleteAsync<Product>(id);
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            var conditions = new List<ScanCondition>();
            return await _context.ScanAsync<Product>(conditions).GetRemainingAsync();
        }
    }
}

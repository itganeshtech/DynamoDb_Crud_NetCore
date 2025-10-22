
using ProductDIExample.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductDIExample.Services
{
    public interface IDataService
    {
        Task SaveProductAsync(Product product);
        Task<Product> GetProductAsync(string id);
        Task DeleteProductAsync(string id);
        Task<List<Product>> GetAllProductsAsync();
    }
}

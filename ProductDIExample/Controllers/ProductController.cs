using Microsoft.AspNetCore.Mvc;
using ProductDIExample.Models;
using ProductDIExample.Services;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProductDIExample.Controllers
{
    public class ProductController : Controller
    {
        private readonly IDataService _dataService;

        public ProductController(IDataService dataService)
        {
            _dataService = dataService;
        }

        // Show all products in a Razor view (GET: /Product)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _dataService.GetAllProductsAsync();
            return View(products); // Returns the View: Views/Product/Index.cshtml
        }

        // View details of a single product (GET: /Product/Details/{id})
        [HttpGet("Product/Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var product = await _dataService.GetProductAsync(id);
            if (product == null)
                return NotFound("Product not found.");
            return View(product);
        }

        // Create product (form + API)
        [HttpGet("Product/Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Product/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                await _dataService.SaveProductAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // Delete product (from view)
        [HttpPost("Product/Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            await _dataService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // REST API endpoints (for Postman / frontend apps)
        [HttpPost("api/Product")]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            await _dataService.SaveProductAsync(product);
            return Ok("Product saved successfully.");
        }

        [HttpGet("api/Product/{id}")]
        public async Task<IActionResult> GetProduct(string id)
        {
            var product = await _dataService.GetProductAsync(id);
            if (product == null)
                return NotFound("Product not found.");
            return Ok(product);
        }

        [HttpGet("api/Product")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _dataService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpDelete("api/Product/{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            await _dataService.DeleteProductAsync(id);
            return Ok("Product deleted successfully.");
        }
    }
}

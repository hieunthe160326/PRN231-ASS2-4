using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using eStoreWebMVC.Controllers;
using eStoreWebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eStoreAPI.Views
{
    public class ProductsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProductsController> _logger;
        private readonly string _apiProductUrl = "http://localhost:5273/odata/Products";
        private readonly string _apiCategoryUrl = "http://localhost:5273/odata/Categories";

        public ProductsController(IHttpClientFactory httpClientFactory, ILogger<ProductsController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<T> GetApi<T>(string apiUrl, bool needValue)
        {
            // Create an HttpClient instance from the factory
            var httpClient = _httpClientFactory.CreateClient();

            // Send a GET request to the API with the query string
            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                var content = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                if (needValue)
                {
                    using (JsonDocument document = JsonDocument.Parse(content))
                    {
                        // Extract the "value" property as a JSON element
                        if (document.RootElement.TryGetProperty("value", out var valueProperty))
                        {
                            // Deserialize the "value" property into a List of objects of type T

                            var result = JsonSerializer.Deserialize<T>(valueProperty.GetRawText(), options);

                            return result;
                        }
                    }
                }
                else
                {
                    var result = JsonSerializer.Deserialize<T>(content, options);
                    return result;
                }

                // Handle the case where the "value" property is not found
                throw new Exception("The 'value' property was not found in the JSON response.");
            }
            else
            {
                // Handle the error gracefully or log it
                var errorMessage = $"Failed to fetch data from API. Status code: {response.StatusCode}";
                throw new Exception(errorMessage);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Index(string searchString, string searchType)
        {
            var apiUrl = _apiProductUrl;
            // Define query parameters
            if (searchType.Equals("name"))
            {
                apiUrl += $"?$filter=contains(ProductName,'{searchString}')";
            }
            else
            {
                apiUrl += $"?$filter=UnitPrice eq {searchString}";
            }
            return View(await GetApi<List<Product>>(apiUrl, true));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await GetApi<List<Product>>(_apiProductUrl, true));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var product = await GetApi<Product>($"{_apiProductUrl}({id})", false);

                if (product == null)
                {
                    return NotFound();
                }

                return View(product);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> Create()
        {
            ViewData["CategoryId"] = new SelectList(await GetApi<List<Category>>(_apiCategoryUrl, true), "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                // Create a dictionary to hold form data
                var formData = new Dictionary<string, string>
                {
                    { nameof(product.ProductName), product.ProductName },
                    { nameof(product.Weight), product.Weight.ToString() },
                    { nameof(product.UnitPrice), product.UnitPrice.ToString() },
                    { nameof(product.UnitInStock), product.UnitInStock.ToString() },
                    { nameof(product.CategoryId), product.CategoryId.ToString() }
                };

                // Create FormUrlEncodedContent from the dictionary
                var content = new FormUrlEncodedContent(formData);
                var response = await httpClient.PostAsync(_apiProductUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Error");
                }
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var product = await GetApi<Product>($"{_apiProductUrl}({id})", false);
                ViewData["CategoryId"] = new SelectList(await GetApi<List<Category>>(_apiCategoryUrl, true), "CategoryId", "CategoryName", product.CategoryId);
                if (product == null)
                {
                    return NotFound();
                }

                return View(product);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                // Create a dictionary to hold form data
                var formData = new Dictionary<string, string>
                {
                    { nameof(product.ProductId), product.ProductId.ToString() },
                    { nameof(product.ProductName), product.ProductName },
                    { nameof(product.Weight), product.Weight.ToString() },
                    { nameof(product.UnitPrice), product.UnitPrice.ToString() },
                    { nameof(product.UnitInStock), product.UnitInStock.ToString() },
                    { nameof(product.CategoryId), product.CategoryId.ToString() }
                };

                // Create FormUrlEncodedContent from the dictionary
                var content = new FormUrlEncodedContent(formData);
                var response = await httpClient.PutAsync($"{_apiProductUrl}({product.ProductId})", content);

                if (response.IsSuccessStatusCode)
                {
                    // Handle successful update, e.g., redirect to Index or show a success message
                    return RedirectToAction("Index");
                }
                else
                {
                    // Handle the error case, e.g., show an error message
                    return View("Error");
                }
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var product = await GetApi<Product>($"{_apiProductUrl}({id})", false);

                if (product == null)
                {
                    return NotFound();
                }

                return View(product);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.DeleteAsync($"{_apiProductUrl}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    // Handle successful deletion, e.g., redirect to Index or show a success message
                    return RedirectToAction("Index");
                }
                else
                {
                    // Handle the error case, e.g., show an error message
                    return View("Error");
                }
            }
            catch (Exception)
            {
                return View("Error");
            }
        }
    }
}

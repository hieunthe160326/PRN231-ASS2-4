using eStoreWebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace eStoreWebMVC.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OrdersController> _logger;
        private readonly string _apiOrderDetailUrl = "http://localhost:5273/odata/OrderDetails";
        private readonly string _apiProductUrl = "http://localhost:5273/odata/Products";

        public OrderDetailsController(IHttpClientFactory httpClientFactory, ILogger<OrdersController> logger)
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

        // GET: OrderDetails/Create
        public async Task<IActionResult> Create(int orderId)
        {
            ViewData["OrderId"] = orderId;
            ViewData["ProductId"] = new SelectList(await GetApi<List<Product>>(_apiProductUrl,true), "ProductId", "ProductName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,ProductId,UnitPrice,Quantity,Discount")] OrderDetail orderDetail)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                // Create a dictionary to hold form data
                var formData = new Dictionary<string, string>
                {
                    { nameof(orderDetail.OrderId), orderDetail.OrderId.ToString() },
                    { nameof(orderDetail.ProductId), orderDetail.ProductId.ToString() },
                    { nameof(orderDetail.UnitPrice), orderDetail.UnitPrice.ToString() },
                    { nameof(orderDetail.Quantity), orderDetail.Quantity.ToString() },
                    { nameof(orderDetail.Discount), orderDetail.Discount.ToString() }
                };

                // Create FormUrlEncodedContent from the dictionary
                var content = new FormUrlEncodedContent(formData);
                var response = await httpClient.PostAsync(_apiOrderDetailUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Edit", "Orders", new { id = orderDetail.OrderId });
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


        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var orderDetail = await GetApi<OrderDetail>($"{_apiOrderDetailUrl}({id})", false);

                if (orderDetail == null)
                {
                    return NotFound();
                }

                return View(orderDetail);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var orderDetail = await GetApi<OrderDetail>($"{_apiOrderDetailUrl}({id})", false);
                var response = await httpClient.DeleteAsync($"{_apiOrderDetailUrl}({id})");

                if (response.IsSuccessStatusCode)
                {
                    // Handle successful deletion, e.g., redirect to Index or show a success message
                    return RedirectToAction("Edit", "Orders", new { id = orderDetail.OrderId });
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

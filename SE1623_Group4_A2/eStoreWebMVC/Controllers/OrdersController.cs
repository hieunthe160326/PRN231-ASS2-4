using eStoreWebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace eStoreWebMVC.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OrdersController> _logger;
        private readonly string _apiOrderUrl = "http://localhost:5273/odata/Orders";
        private readonly string _apiMemberUrl = "http://localhost:5273/odata/Members";

        public OrdersController(IHttpClientFactory httpClientFactory, ILogger<OrdersController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var orders = await GetApi<List<Order>>(_apiOrderUrl, true);

            return View(orders);
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

        // GET: OrderDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await GetApi<Order>($"{_apiOrderUrl}({id})?$expand=OrderDetails($expand=Product)", false);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: OrderDetails/Create
        public async Task<IActionResult> Create()
        {
            ViewData["MemberId"] = new SelectList(await GetApi<List<Member>>(_apiMemberUrl, true), "MemberId", "MemberId");           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,MemberId,OrderDate,RequiredDate,ShippedDate,Freight")] Order order)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                // Create a dictionary to hold form data
                var formData = new Dictionary<string, string>
                    {
                        { nameof(order.OrderId), order.OrderId.ToString() },
                        { nameof(order.MemberId), order.MemberId.ToString() },
                        { nameof(order.OrderDate), order.OrderDate.ToString("MM/dd/yyyy") },
                        { nameof(order.RequiredDate), order.RequiredDate?.ToString("MM/dd/yyyy") },
                        { nameof(order.ShippedDate), order.ShippedDate.ToString("MM/dd/yyyy") },
                        { nameof(order.Freight), order.Freight?.ToString() }
                    };

                // Create FormUrlEncodedContent from the dictionary
                var content = new FormUrlEncodedContent(formData);

                var response = await httpClient.PostAsync(_apiOrderUrl, content);

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

            var order = await GetApi<Order>($"{_apiOrderUrl}({id})?$expand=OrderDetails($expand=Product)", false);

            if (order == null)
            {
                return NotFound();
            }

            ViewData["MemberId"] = new SelectList(await GetApi<List<Member>>(_apiMemberUrl, true), "MemberId", "MemberId", id);
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,MemberId,OrderDate,RequiredDate,ShippedDate,Freight")] Order order)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                // Create a dictionary to hold form data
                var jsonData = JsonSerializer.Serialize(order);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(_apiOrderUrl, content);

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

        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var product = await GetApi<Order>($"{_apiOrderUrl}({id})", false);

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

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.DeleteAsync($"{_apiOrderUrl}({id})");

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

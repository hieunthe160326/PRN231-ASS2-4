using eStoreWebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace eStoreWebMVC.Controllers
{
    public class MembersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MembersController> _logger;
        private readonly string _apiMemberUrl = "http://localhost:5273/odata/Members";

        public MembersController(IHttpClientFactory httpClientFactory, ILogger<MembersController> logger)
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

        public async Task<IActionResult> Index()
        {
            return View(await GetApi<List<Member>>(_apiMemberUrl, true));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await GetApi<Member>($"{_apiMemberUrl}({id})", false);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Email,CompanyName,City,Country,Password")] Member member)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                // Create a dictionary to hold form data
                var formData = new Dictionary<string, string>
                {
                    { nameof(member.Email), member.Email },
                    { nameof(member.CompanyName), member.CompanyName },
                    { nameof(member.City), member.City },
                    { nameof(member.Country), member.Country },
                    { nameof(member.Password), member.Password },
                };

                // Create FormUrlEncodedContent from the dictionary
                var content = new FormUrlEncodedContent(formData);
                var response = await httpClient.PostAsync(_apiMemberUrl, content);

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

        // GET: Members/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await GetApi<Member>($"{_apiMemberUrl}({id})", false);

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberId,Email,CompanyName,City,Country,Password")] Member member)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                // Create a dictionary to hold form data
                var formData = new Dictionary<string, string>
                {
                    { nameof(member.MemberId), member.MemberId.ToString() },
                    { nameof(member.Email), member.Email },
                    { nameof(member.CompanyName), member.CompanyName },
                    { nameof(member.City), member.City },
                    { nameof(member.Country), member.Country },
                    { nameof(member.Password), member.Password },
                };

                // Create FormUrlEncodedContent from the dictionary
                var content = new FormUrlEncodedContent(formData);
                var response = await httpClient.PutAsync($"{_apiMemberUrl}/{id}", content);

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

        // GET: Members/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var product = await GetApi<Member>($"{_apiMemberUrl}({id})", false);

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
                var response = await httpClient.DeleteAsync($"{_apiMemberUrl}({id})");

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

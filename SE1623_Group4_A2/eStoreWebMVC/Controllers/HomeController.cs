using Client_eStore.Helper;
using eStoreAPI.Models;
using eStoreWebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace eStoreWebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient client;
        private string api;

        public HomeController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            api = "https://localhost:5273/Product/";
        }

        public async Task<IActionResult> Index()
        {
            var list = await client.GetApi<IEnumerable<Product>>(api + "all");
            return View(list);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
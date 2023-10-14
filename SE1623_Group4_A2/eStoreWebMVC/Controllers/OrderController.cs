using eStoreWebMVC.Helper;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using eStoreAPI.Models;
namespace eStoreWebMVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly HttpClient client;
        private string api;

        public OrderController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            contentType.Parameters.Add(new NameValueHeaderValue("odata.metadata", "none"));
            client.DefaultRequestHeaders.Accept.Add(contentType);
            api = "https://localhost:5072/odata/order";
        }

        public async Task<IActionResult> ViewOrder()
        {
            var list = await client.GetApi<IEnumerable<Order>>(api);

            return View(list);
        }
    }
}

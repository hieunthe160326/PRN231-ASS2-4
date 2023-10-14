using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace eStoreWebMVC.Controllers
{
    public class MemberController : Controller
    {
        private readonly HttpClient client;
        private string api;

        public MemberController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            contentType.Parameters.Add(new NameValueHeaderValue("odata.metadata", "none"));
            client.DefaultRequestHeaders.Accept.Add(contentType);
            api = "https://localhost:5072/odata/customers";
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}

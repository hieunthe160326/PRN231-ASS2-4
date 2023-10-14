using eStoreWebMVC.Helper;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using eStoreAPI.Models;

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
            api = "https://localhost:5072/odata/member";
        }
        /*public IActionResult Index()
        {
            return View();
        }*/
        
        
        public async Task<IActionResult> Index()
        {
            var list = await client.GetApi<IEnumerable<Member>>(api);

            return View(list);
        }
        
    }
}
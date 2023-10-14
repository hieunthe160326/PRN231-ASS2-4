using eStoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace eStoreAPI.Controllers
{
    public class MembersController : Controller
    {
        private readonly EStoreContext _context;

        public MembersController(EStoreContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.Members);
        }
    }
}

using eStoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace eStoreAPI.Controllers
{
    public class CategoriesController : ODataController
    {
        private readonly EStoreContext _context;

        public CategoriesController(EStoreContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.Categories);
        }
    }
}

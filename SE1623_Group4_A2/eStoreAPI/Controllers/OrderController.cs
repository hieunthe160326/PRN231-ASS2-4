using eStoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Net;

namespace eStoreAPI.Controllers
{
    [Route("Order")]
    [ApiController]
    public class OrderController : ODataController
    {
        private readonly EStoreContext _context;

        public OrderController(EStoreContext context)
        {
            this._context = context;
        }

        [EnableQuery]
        [HttpGet]
        public IQueryable<Order> Get()
        {
            return _context.Orders;
        }

        [EnableQuery]
        [HttpGet("{key}")]
        public IActionResult Get(int key)
        {
            return Ok(_context.Orders.Where(m => m.OrderId == key));
        }

        [HttpDelete("{key}")]
        public IActionResult Delete([FromODataUri] int key)
        {
            Order order = _context.Orders.FirstOrDefault(m => m.OrderId == key);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();

            return Ok();
        }
    }
}

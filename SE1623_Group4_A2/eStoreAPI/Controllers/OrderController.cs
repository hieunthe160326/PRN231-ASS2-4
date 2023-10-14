using eStoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
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
        public SingleResult<Order> Get([FromODataUri] int key)
        {
            IQueryable<Order> result = _context.Orders.Where(m => m.OrderId == key);
            return SingleResult.Create(result);
        }

        [HttpDelete("{key}")]
        public IActionResult Delete([FromODataUri] int key)
        {
            var order = _context.Orders.FirstOrDefault(m => m.MemberId == key);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();

            return StatusCode((int)HttpStatusCode.NoContent);
        }
    }
}

using eStoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace eStoreAPI.Controllers
{
    [Route("Order")]
    [ApiController]
    public class OrderController : ODataController
    {
        private readonly EStoreContext context;

        public OrderController(EStoreContext context)
        {
            this.context = context;
        }

        [EnableQuery]
        [HttpGet]
        public IQueryable<Order> GetOrders()
        {
            return context.Orders.AsQueryable();
        }

        [EnableQuery]
        [HttpGet("{id}")]
        public SingleResult<Order> GetOrder([FromODataUri] int id)
        {
            var order = context.Orders.Where(o => o.OrderId == id);
            return SingleResult.Create(order);
        }

        [EnableQuery]
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder([FromODataUri] int id)
        {
            var order = context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            context.Orders.Remove(order);
            context.SaveChanges();

            return NoContent();
        }
    }
}

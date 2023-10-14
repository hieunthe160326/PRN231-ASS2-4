using eStoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace eStoreAPI.Controllers
{
    public class OrdersController : ODataController
    {
        private readonly EStoreContext _context;

        public OrdersController(EStoreContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.Orders);
        }

        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            var product = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == key);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        [EnableQuery]
        public async Task<IActionResult> Post([FromForm] Order order)
        {
            if (!ModelState.IsValid)
            {               
                return BadRequest(ModelState);
            }

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return Created(order);
        }

        [HttpPut]
        [EnableQuery]
        public async Task<IActionResult> Put([FromODataUri] int key, [FromForm] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != order.OrderId)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Updated(order);
        }

        [HttpDelete]
        [EnableQuery]
        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            var order = await _context.Orders.FindAsync(key);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}


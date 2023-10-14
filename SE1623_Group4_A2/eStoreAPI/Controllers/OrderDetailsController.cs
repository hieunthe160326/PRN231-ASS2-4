using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eStoreAPI.Models;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace eStoreAPI.Controllers
{
    public class OrderDetailsController : ODataController
    {
        private readonly EStoreContext _context;

        public OrderDetailsController(EStoreContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.OrderDetails);
        }

        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(key);
            if (orderDetail == null)
            {
                return NotFound();
            }
            return Ok(orderDetail);
        }

        [HttpPost]
        [EnableQuery]
        public async Task<IActionResult> Post([FromForm] OrderDetail orderDetail)
        {
            ModelState.Remove(nameof(orderDetail.Order));
            ModelState.Remove(nameof(orderDetail.Product));

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return BadRequest(ModelState);
            }

            await _context.OrderDetails.AddAsync(orderDetail);
            await _context.SaveChangesAsync();
            return Created(orderDetail);
        }

        [HttpPut]
        [EnableQuery]
        public async Task<IActionResult> Put([FromODataUri] int key, [FromBody] OrderDetail orderDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != orderDetail.OrderDetailId)
            {
                return BadRequest();
            }

            _context.Entry(orderDetail).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Updated(orderDetail);
        }

        [HttpDelete]
        [EnableQuery]
        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(key);
            if (orderDetail == null)
            {
                return NotFound();
            }

            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}


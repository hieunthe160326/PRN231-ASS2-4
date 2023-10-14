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
    [Route("odata/OrderDetails")]
    public class OrderDetailsODataController : ODataController
    {
        private readonly EStoreContext _context;

        public OrderDetailsODataController(EStoreContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.OrderDetails);
        }

        [EnableQuery]
        public IActionResult Get([FromODataUri] int key)
        {
            var orderDetail = _context.OrderDetails.FirstOrDefault(o => o.OrderDetailId == key);
            if (orderDetail == null)
            {
                return NotFound();
            }
            return Ok(orderDetail);
        }

        [HttpPost]
        [EnableQuery]
        public IActionResult Post([FromBody] OrderDetail orderDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.OrderDetails.Add(orderDetail);
            _context.SaveChanges();
            return Created(orderDetail);
        }

        [HttpPut]
        [EnableQuery]
        public IActionResult Put([FromODataUri] int key, [FromBody] OrderDetail orderDetail)
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
            _context.SaveChanges();
            return Updated(orderDetail);
        }

        [HttpDelete]
        [EnableQuery]
        public IActionResult Delete([FromODataUri] int key)
        {
            var orderDetail = _context.OrderDetails.Find(key);
            if (orderDetail == null)
            {
                return NotFound();
            }

            _context.OrderDetails.Remove(orderDetail);
            _context.SaveChanges();
            return NoContent();
        }
    }
}


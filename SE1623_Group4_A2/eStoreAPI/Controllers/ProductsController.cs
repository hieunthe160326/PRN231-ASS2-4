using eStoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace eStoreAPI.Controllers
{
    public class ProductsController : ODataController
    {
        private readonly EStoreContext _context;

        public ProductsController(EStoreContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.Products);
        }

        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            var product = await _context.Products.FindAsync(key);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        [EnableQuery]
        public async Task<IActionResult> Post([FromForm] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return Created(product);
        }

        [HttpPut]
        [EnableQuery]
        public async Task<IActionResult> Put([FromODataUri] int key, [FromForm] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != product.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Updated(product);
        }

        [HttpDelete]
        [EnableQuery]
        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            var product = await _context.Products.FindAsync(key);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

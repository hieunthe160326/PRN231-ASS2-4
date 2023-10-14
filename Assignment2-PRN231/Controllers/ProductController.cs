using Assignment2_PRN231.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Assignment2_PRN231.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ODataController
    {
        private readonly EStoreContext context;

        public ProductController(EStoreContext _context)
        {
            context = _context;
        }
        [EnableQuery]
        [HttpGet]
        public IQueryable<Product> Get()
        {
            return context.Products;
        }

        [EnableQuery]
        [HttpGet("{key}")]
        public IActionResult Get(int key)
        {
            return Ok(context.Products.FirstOrDefault(a => a.ProductId == key));
        }

        [HttpPost]
        public IActionResult Post([FromBody] Product product)
        {
            context.Products.Add(product);
            context.SaveChanges();

            return Created(product);
        }

        [HttpPut("{key}")]
        public IActionResult Put([FromODataUri] int key, [FromBody] Product product)
        {

          Product p = context.Products.FirstOrDefault(a => a.ProductId == key);
            if(p != null)
            {
                context.Entry(product).State = EntityState.Modified;
                context.SaveChanges();
            }

            return Updated(product);
        }

        [HttpDelete("{key}")]
        public IActionResult Delete([FromODataUri] int key)
        {
            Product p = context.Products.FirstOrDefault(a => a.ProductId == key);
            if (p == null)
            {
                return NotFound();
            }

            context.Products.Remove(p);
            context.SaveChanges();

            return Ok();
        }

    }
}
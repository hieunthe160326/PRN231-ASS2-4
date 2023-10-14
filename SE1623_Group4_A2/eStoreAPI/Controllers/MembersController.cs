using eStoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace eStoreAPI.Controllers
{
    public class MembersController : ODataController
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

        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            var product = await _context.Members.FindAsync(key);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        [EnableQuery]
        public async Task<IActionResult> Post([FromForm] Member member)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.Members.AddAsync(member);
            await _context.SaveChangesAsync();
            return Created(member);
        }

        [HttpPut]
        [EnableQuery]
        public async Task<IActionResult> Put([FromODataUri] int key, [FromForm] Member member)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != member.MemberId)
            {
                return BadRequest();
            }

            _context.Entry(member).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Updated(member);
        }

        [HttpDelete]
        [EnableQuery]
        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            var member = await _context.Members.FindAsync(key);
            if (member == null)
            {
                return NotFound();
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

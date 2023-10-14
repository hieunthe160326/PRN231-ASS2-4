using eStoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Net;

namespace eStoreAPI.Controllers
{
    public class MemberController : ODataController
    {
        private readonly EStoreContext _context;

        public MemberController(EStoreContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IQueryable<Member> Get()
        {
            return _context.Members;
        }

        [EnableQuery]
        public SingleResult<Member> Get([FromODataUri] int key)
        {
            IQueryable<Member> result = _context.Members.Where(m => m.MemberId == key);
            return SingleResult.Create(result);
        }

        public IActionResult Post([FromBody] Member member)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Members.Add(member);
            _context.SaveChanges();

            return Created(member);
        }

        public IActionResult Patch([FromODataUri] int key, [FromBody] Delta<Member> member)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = _context.Members.FirstOrDefault(m => m.MemberId == key);
            if (entity == null)
            {
                return NotFound();
            }

            member.Patch(entity);
            _context.SaveChanges();

            return Updated(entity);
        }

        public IActionResult Delete([FromODataUri] int key)
        {
            var member = _context.Members.FirstOrDefault(m => m.MemberId == key);
            if (member == null)
            {
                return NotFound();
            }

            _context.Members.Remove(member);
            _context.SaveChanges();

            return StatusCode((int)HttpStatusCode.NoContent);
        }
    }
}

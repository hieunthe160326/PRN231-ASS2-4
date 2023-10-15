using eStoreAPI.DTOs;
using eStoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace eStoreAPI.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ODataController
    {
        private readonly EStoreContext context;

        public AccountController(EStoreContext context)
        {
            this.context = context;
        }

        [EnableQuery]
        [HttpGet]
        public IQueryable<Member> GetAccount()
        {
            return context.Members.AsQueryable();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Member member)
        {
            try
            {
                // Kiểm tra xem email đã tồn tại hay chưa
                if (await context.Members.AnyAsync(m => m.Email == member.Email))
                {
                    return BadRequest("Email đã tồn tại.");
                }else
                context.Members.Add(member);
                await context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi server: " + ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginInfo info)
        {
            var member = await context.Members.SingleOrDefaultAsync(m => m.Email == info.Email);

            if (member == null || member.Password != info.Password)
            {
                return Unauthorized("Email hoặc mật khẩu không đúng.");
            }
            else
            {
                return Ok(member);
            }
            
            // Xử lý đăng nhập thành công và tạo token JWT tương tự như trước.
        }

    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using TechWizWebApp.Data;
using TechWizWebApp.Interfaces;
using TechWizWebApp.RequestModels;

namespace TechWizWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthAdminController : ControllerBase
    {
        private readonly IAuthAdmin _authAdmin;
        private readonly DecorVistaDbContext _context;

        public AuthAdminController(IAuthAdmin authAdmin,
            DecorVistaDbContext context)
        {
            _authAdmin = authAdmin;
            _context = context;
        }

        [HttpPost]
        [Route("admin_login")]
        public async Task<IActionResult> AdminLogin([FromForm] RequestLogin requestLogin)
        {

            var customResult = await _authAdmin.AdminLogin(requestLogin.Email, requestLogin.Password);

            return Ok(customResult);
        }

        [HttpGet]
        [Route("admin")]
        [Authorize(Roles = "admin, designer")]
        public async Task<IActionResult> GetAdmin()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
            int.TryParse(idClaim, out int userId);

            var customResult = await _authAdmin.GetAdmin(userId);

            return Ok(customResult);
        }

        [HttpPut]
        [Route("reset_password")]
        [Authorize(Roles = "admin, designer")]
        public async Task<IActionResult> GetAdmin([FromForm] PasswordReset passwordReset)
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
            int.TryParse(idClaim, out int userId);

            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(p => p.id == userId);

                if (BCrypt.Net.BCrypt.Verify(passwordReset.previous, user.password ))
                {
                    user.password = BCrypt.Net.BCrypt.HashPassword(passwordReset.newPassword);

                    _context.Users.Update(user);

                    await _context.SaveChangesAsync();

                    return Ok(new CustomResult(200, "Ok", null));
                }

                return Ok(new CustomResult(403, "Wrong password", null));
            }
            catch (Exception ex)
            {
                return Ok(new CustomResult(400, "Bad request", ex.Message));
            }


        }

        public class PasswordReset
        {
            public string previous { get; set; }
            public string newPassword { get; set; }
        }
    }
}

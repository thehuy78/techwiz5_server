using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechWizWebApp.Data;
using TechWizWebApp.Interface;

using TechWizWebApp.RepositotyCustomer;
using TechWizWebApp.Services;

namespace TechWizWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthUserController : ControllerBase
    {
        private readonly IAuthFE _auth;
        private readonly IMailService _mailService;
        private readonly DecorVistaDbContext _context;
        public AuthUserController(IAuthFE auth, IMailService mailService, DecorVistaDbContext decorVistaDbContext)
        {
            _auth = auth;
            _mailService = mailService;
            _context = decorVistaDbContext;
        }


        [HttpPost("Login")]
        public async Task<ActionResult> GetById(UserLogin login)
        {
            var result = await _auth.Login(login);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }



        [HttpPost("LoginGoogle")]
        public async Task<ActionResult> LoginGoogle(UserDto login)
        {
            var result = await _auth.LoginGoogle(login);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);

            }
        }


                [HttpPost("Register")]
        public async Task<ActionResult> Register(UserDto user)
        {
            var result = await _auth.Register(user);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpPut("ForgortPassword")]
        public async Task<ActionResult> ForgotPassword([FromForm] ForgotPasswordRequest request)
        {
            //kiem tra email da duoc tao chua
            var user = await _context.Users.Include(u => u.userdetails).Where(u => u.email == request.email).FirstOrDefaultAsync();
            if (user == null)
            {
                return Ok(new CustomResult
                {
                    data = null,
                    Message = "Email not found",
                    Status = 400
                });
            }

            //Gui mail

            //encond userId
            var passwordReset = Guid.NewGuid().ToString();
            DateTime expiryTime = DateTime.UtcNow.AddHours(1);
            string combinedString = $"{user.id}|{expiryTime:o}|{passwordReset}";
            string encodedUserId = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(combinedString));
            string frontendUrl = $"http://localhost:3000/techwiz5_client/forgotpassword/{encodedUserId}";



            string imgUrl = "<img src=\"https://firebasestorage.googleapis.com/v0/b/techwizwebapp.appspot.com/o/Images%2Fef8273f5-d9bf-4b95-8602-5f1de021201a.png?alt=media&token=c9257fe5-ce0a-46d3-b541-a1333d0c3f58\" alt=\"Image\" width=\"200\" height=\"160\" />";
            string resetPasswordUrl = $"https://localhost:7229/api/AuthUser/ResetPassword?userId={user.id}"; // lay cai nay
            string emailContent = @$"
Dear {user?.userdetails?.first_name} {user?.userdetails?.last_name},  <div></div>

We have received a request to reset the password for your account at DecorVista.   <div></div>

Your temporary password is : {passwordReset}  <div></div>
 <div> Expired in : <strong  color=""red""> {expiryTime} </strong> </div>
If you accept to reset please click link here:  <a href=""{frontendUrl}"">Click here</a> <div></div>

Best regards,
<div>{imgUrl}</div>

";
            await _mailService.SendMailAsync(user?.email, "Forgot Password Information", emailContent);


            return Ok(new CustomResult
            {
                data = null,
                Message = "Oke",
                Status = 200
            });
        }

        [HttpGet("ResetPassword")]
        public async Task<ActionResult> ResetPassword(int userId, string newPassword)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.id == userId);
            user.password = BCrypt.Net.BCrypt.HashPassword(newPassword.Trim());
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return Ok(new CustomResult
            {
                Status = 200,
                data = null,
                Message = "Oke"
            });
        }


        public class ForgotPasswordRequest
        {
            public string? email { get; set; }
        }
    }
}

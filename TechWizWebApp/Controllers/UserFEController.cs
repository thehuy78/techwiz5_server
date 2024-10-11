using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.InterfaceCustomer;
using TechWizWebApp.RepositotyCustomer;
namespace TechWizWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserFEController : ControllerBase
    {
        private IUserFE _user;
        private readonly IConfiguration _config;
        private readonly DecorVistaDbContext _context;
        public UserFEController(IUserFE user, DecorVistaDbContext decorVistaDbContext, IConfiguration configuration)
        {
            _config = configuration;
            _user = user;
            _context = decorVistaDbContext;
        }
        [HttpGet("GetByEmail/{email}")]
        public async Task<ActionResult> GetByEmail(string email)
        {
            var result = await _user.GetByEmail(email);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpPut("UpdateProfile")]
        public async Task<ActionResult> UpdateProfile([FromForm] UserRequest us)
        {
            try
            {
                var user = _context.Users.Include(u => u.userdetails).Where(u => u.id == us.user_id).FirstOrDefault();
                //user.email = us.email;

                UserDetails userDetails = new UserDetails();
                userDetails.first_name = us.first_name;
                userDetails.last_name = us.last_name;
                userDetails.contact_number = us.phone;
                userDetails.role = "customer";

                user.userdetails = userDetails;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

               

              
                return Ok(new CustomResult
                {
                    data = GenerateJSONWebToken(user),
                    Message = "oke",
                    Status = 200
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new CustomResult
                {
                    data = null,
                    Message = "oke",
                    Status = 400
                });
            }
        }

        [HttpPut("UpdatePassword")]
        public async Task<ActionResult> UpdatePassord([FromForm] UpdatePasswordReponse up)
        {
            try
            {
                var result = await _user.UpdatePassword(up);
                return Ok(result);

            }catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }





        private string GenerateJSONWebToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
               new Claim("email",user.email),
               new Claim("first_name",user.userdetails.first_name),
               new Claim("last_name",user.userdetails.last_name),
               new Claim("id",user.id.ToString()),
               new Claim(ClaimTypes.Email, user.email)
            };

            var token = new JwtSecurityToken(
                _config["JwtSettings:Issuer"],
                _config["JwtSettings:Audience"],
                claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }




    public class UserRequest
        {
            public int user_id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string phone { get; set; }
            //public string email { get; set; }
        }
     
    }


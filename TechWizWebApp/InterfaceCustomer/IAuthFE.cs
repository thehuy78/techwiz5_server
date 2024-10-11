using TechWizWebApp.Domain;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TechWizWebApp.Data;
using CustomResult = TechWizWebApp.Data.CustomResult;
using Microsoft.EntityFrameworkCore;
using TechWizWebApp.RepositotyCustomer;
using TechWizWebApp.Services;

namespace TechWizWebApp.Interface
{
    public interface IAuthFE
    {
        Task<CustomResult> Login(UserLogin login);
        Task<CustomResult> Register(UserDto user);
        Task<CustomResult> LoginGoogle(UserDto user);
    }
//    public class AuthLogin : IAuthFE
//    {
//        private readonly IConfiguration _config;
//        private readonly DecorVistaDbContext _context;
//        private readonly IMailService _mailService;
//        public AuthLogin(IConfiguration configuration, DecorVistaDbContext decorVistaDbContext, IMailService mailService)
//        {
//            _config = configuration;
//            _context = decorVistaDbContext;
//            _mailService = mailService;
//        }

//        public async Task<CustomResult> Login(UserLogin login)
//        {

//            var user = await _context.Users.Include(u => u.userdetails).Where(u => u.email == login.Email).FirstOrDefaultAsync();

//            if (user == null)
//            {
//                return new CustomResult()
//                {
//                    data = null,
//                    Status = 300,
//                    Message = "User not exist"
//                };
//            }

//            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(login.Password, user.password);
//            if (isPasswordValid)
//            {

//                return new CustomResult()
//                {
//                    data = GenerateJSONWebToken(user),
//                    Status = 200,

//                };

//            }

//            return new CustomResult()
//            {
//                data = null,
//                Status = 300,
//                Message = "Password Wrong"
//            };
//        }

//        public async Task<CustomResult> LoginGoogle(UserDto user)
//        {
//            try
//            {
//                //check trung
//                var userOld = await _context.Users.Include(u => u.userdetails).Where(u => u.email == user.email).FirstOrDefaultAsync();
//                if (userOld != null)
//                {
//                    return new CustomResult
//                    {
//                        data = GenerateJSONWebToken(userOld),
//                        Message = "Login",
//                        Status = 200
//                    };
//                }


//                var u = new User();

//                u.email = user.email;

//                var userDetails = new UserDetails();
//                userDetails.first_name = user.userdetails.first_name;
//                userDetails.last_name = user.userdetails.last_name;


//                u.userdetails = userDetails;



//                _context.Users.Add(u);
//                await _context.SaveChangesAsync();
//                return new CustomResult
//                {
//                    data = GenerateJSONWebToken(u),
//                    Message = "Login",
//                    Status = 200
//                };
//            }
//            catch
//            {
//                return new CustomResult
//                {
//                    data = null,
//                    Message = "Login fails",
//                    Status = 400
//                };
//            }

//        }

//        public async Task<CustomResult> Register(UserDto userCreate)
//        {
//            try
//            {

//                //check trung
//                var userOld = await _context.Users.Where(u => u.email == userCreate.email).FirstOrDefaultAsync();
//                if (userOld != null)
//                {
//                    return new CustomResult
//                    {
//                        data = null,
//                        Message = "Email duplicate",
//                        Status = 400
//                    };
//                }


//                var user = new User();

//                user.email = userCreate.email;

//                var userDetails = new UserDetails();
//                userDetails.first_name = userCreate.userdetails.first_name;
//                userDetails.last_name = userCreate.userdetails.last_name;


//                user.userdetails = userDetails;


//                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userCreate.password);
//                user.password = hashedPassword;
//                _context.Users.Add(user);
//                await _context.SaveChangesAsync();

//                //gui mail 
//                string imgUrl = "<img src=\"https://firebasestorage.googleapis.com/v0/b/techwizwebapp.appspot.com/o/Images%2Fef8273f5-d9bf-4b95-8602-5f1de021201a.png?alt=media&token=c9257fe5-ce0a-46d3-b541-a1333d0c3f58\" alt=\"Image\" width=\"200\" height=\"160\" />";
//                string webUrl = @"<a href=""http://localhost:3000/tw5"">Decor Vista</a>";
//                string emailContent = @$"
//Subject: Account Registration Confirmation <div></div>

//Dear {userDetails.first_name}  {userDetails.last_name}, <div></div>
 
//Thank you for registering an account at {webUrl}! <div></div>

//Best regards, <div></div>
//<div>{imgUrl}</div>
//";
//                await _mailService.SendMailAsync(user.email, "Register Information", emailContent);

//                return new CustomResult()
//                {
//                    Status = 200,
//                    Message = "Create Success!"
//                };

//            }

//            catch (Exception ex)
//            {
//                return new CustomResult()
//                {
//                    Status = 400,
//                    Message = "Create Fail!"
//                };
//            }



//        }

//        private string GenerateJSONWebToken(User user)
//        {
//            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
//            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

//            var claims = new[]
//            {
//               new Claim("email",user.email),
//               new Claim("first_name",user.userdetails.first_name),
//               new Claim("last_name",user.userdetails.last_name),
//               new Claim("id",user.id.ToString()),
//               new Claim(ClaimTypes.Email, user.email)
//            };

//            var token = new JwtSecurityToken(
//                _config["JwtSettings:Issuer"],
//                _config["JwtSettings:Audience"],
//                claims,
//                expires: DateTime.Now.AddHours(1),
//                signingCredentials: credentials
//                );

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }
//    }

    //public class UserLogin
    //{
    //    public string Email { get; set; }
    //    public string Password { get; set; }
    //}

    //public class UserDto
    //{
    //    public int id { get; set; }
    //    public string email { get; set; } = string.Empty;
    //    public string password { get; set; } = string.Empty;
    //    public UserDetatailsDto? userdetails { get; set; }
    //}
    //public class UserDetatailsDto
    //{
    //    public int id { get; set; }
    //    public int user_id { get; set; }
    //    public string first_name { get; set; } = string.Empty;
    //    public string last_name { get; set; } = string.Empty;
    //    public string contact_number { get; set; } = string.Empty;
    //    public string address { get; set; } = string.Empty;
    //}
}

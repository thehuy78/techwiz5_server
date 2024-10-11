using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.Interfaces;

namespace TechWizWebApp.Repositories
{
    public class AuthAdminRepo : IAuthAdmin
    {
        private readonly DecorVistaDbContext _context;
        private readonly IConfiguration _config;


        public AuthAdminRepo(DecorVistaDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<CustomResult> AdminLogin(string email, string password)
        {
            try
            {
                var user = await _context.Users.Include(u => u.interiordesigner).Include(u => u.userdetails).Where(u => u.email == email).SingleOrDefaultAsync();

                if (user != null && BCrypt.Net.BCrypt.Verify(password, user.password))
                {
                    if (user.userdetails != null && user.userdetails.role == "admin")
                    {
                        var token = CreateToken(user);

                        return new CustomResult(200, "token", token);
                    }

                    if(user.interiordesigner != null)
                    {
                        if(user.interiordesigner.approved_status != "approved")
                        {
                            return new CustomResult(403, "This account is not approved to become designer", null);
                        }

                        if (user.interiordesigner.status == false)
                        {
                            return new CustomResult(403, "This account is not active", null);
                        }

                        var token = CreateToken(user);

                        return new CustomResult(200, "token", token);


                    }

                    return new CustomResult(401, "Unauthorized", null);

                }

                return new CustomResult(404, "Wrong username or password", null);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Bad Request", ex.Message);
            }
        }

        public string CreateToken(Domain.User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.email),
                new Claim(ClaimTypes.Role, user.userdetails != null ? user.userdetails.role : "designer" ),
                new Claim("Id", user.id.ToString()),
            };

            var token = new JwtSecurityToken(
                    issuer: _config["JwtSettings:Issuer"],
                    audience: _config["JwtSettings:Audience"],
                    signingCredentials: credentials,
                    claims: claims,
                    expires: DateTime.Now.AddDays(7)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<CustomResult> GetAdmin(int userId)
        {
            try
            {
                var admin = await _context.Users.Include(u => u.interiordesigner).Include(u => u.userdetails).SingleOrDefaultAsync(u => u.id == userId);

                if (admin != null)
                {
                    return new CustomResult(200, "Ok", admin);
                }

                return new CustomResult(404, "User not found", null);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Bad Request", ex.Message);
            }
        }

    }
}

using Microsoft.EntityFrameworkCore;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.InterfaceCustomer;

namespace TechWizWebApp.RepositotyCustomer
{
    public class UserFERepo : IUserFE
    {
        private DecorVistaDbContext _db;
        public UserFERepo(DecorVistaDbContext db)
        {
            _db = db;
        }
        public async Task<CustomResult> GetByEmail(string email)
        {
            try
            {
                var data = await _db.UserDetails.SingleOrDefaultAsync(e => e.User.email == email);
                return new CustomResult()
                {
                    Status = 200,
                    data = data
                };
            }
            catch (Exception ex)
            {
                return new CustomResult()
                {
                    Status = 400,
                    Message = ex.Message
                };
            }
        }

        public async Task<CustomResult> UpdatePassword(UpdatePasswordReponse up)
        {
            if (up.newPassword != up.confirmPassowrd)
            {
                return new CustomResult()
                {
                    data = null,
                    Message = "confirm passowrd not match",
                    Status = 400
                };
            }

            var user = await _db.Users.Where(u => u.email == up.email).FirstOrDefaultAsync();

            if (!BCrypt.Net.BCrypt.Verify(up.oldPassword, user.password))
            {
                return new CustomResult()
                {
                    data = null,
                    Message = "old password not correct",
                    Status = 400
                };
            }
            user.password = BCrypt.Net.BCrypt.HashPassword(up.newPassword);
            _db.Users.Update(user);
           await _db.SaveChangesAsync();
            return new CustomResult()
            {
                data = null,
                Message = "Update Success",
                Status = 200
            };
        }

        public async Task<CustomResult> UpdateProfile(UserDetails userDetails)
        {
            try
            {
                var oldData = await _db.UserDetails.SingleOrDefaultAsync(e => e.id == userDetails.id);
                oldData.avatar = userDetails.avatar;
                oldData.address = userDetails.address;
                oldData.role = userDetails.role;
                oldData.first_name = userDetails.first_name;
                oldData.last_name = userDetails.last_name;
                oldData.contact_number = userDetails.contact_number;
                oldData.user_id = userDetails.user_id;
                _db.UserDetails.Update(oldData);
                await _db.SaveChangesAsync();
                return new CustomResult()
                {
                    Status = 200,
                    Message = "Update Success"
                };

            }
            catch (Exception ex)
            {
                return new CustomResult()
                {
                    Status = 400,
                    Message = "Update Fail!"
                };
            }
        }
    }
    public class UpdatePasswordReponse
    {
        public string? email { get; set; }
        public string? oldPassword { get; set; }
        public string? newPassword { get; set; }
        public string? confirmPassowrd { get; set; }
    }
}

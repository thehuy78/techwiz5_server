

using TechWizWebApp.Data;
using TechWizWebApp.Domain;

namespace TechWizWebApp.Interfaces
{
    public interface IAuthAdmin
    {
        public String CreateToken(User user);

        Task<CustomResult> GetAdmin(int userId);

        Task<CustomResult> AdminLogin(String email, String password);
    }
}

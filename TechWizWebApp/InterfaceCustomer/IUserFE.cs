using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.RepositotyCustomer;
namespace TechWizWebApp.InterfaceCustomer
{
    public interface IUserFE
    {

        Task<CustomResult> GetByEmail(string email);

        Task<CustomResult> UpdateProfile(UserDetails userDetails);


        Task<CustomResult> UpdatePassword(UpdatePasswordReponse up);


    }
}

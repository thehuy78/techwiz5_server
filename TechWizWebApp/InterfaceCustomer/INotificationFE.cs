using TechWizWebApp.Data;

namespace TechWizWebApp.InterfaceCustomer
{
    public interface INotificationFE
    {

        Task<CustomResult> GetNotificationByUser(int id);

        Task<CustomResult> ReadAction(int id);


        Task<CustomResult> Deleted(int id);
    }
}

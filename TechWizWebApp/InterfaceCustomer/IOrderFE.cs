using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.RepositotyCustomer;


namespace TechWizWebApp.InterfaceCustomer
{
    public interface IOrderFE
    {
        Task<CustomResult> setOrder(RequestOrder request);

        Task<CustomResult> listOrderByUser(int id);


    }
}
using TechWizWebApp.Data;

namespace TechWizWebApp.InterfaceCustomer
{
    public interface IOrderdetailFE
    {
        Task<CustomResult> getByOrderId(string orderid);
    }
}

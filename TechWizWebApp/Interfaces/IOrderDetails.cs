

using TechWizWebApp.Data;

namespace TechWizWebApp.Interface
{
    public interface IOrderDetails
    {
        Task<CustomResult> GetByOrderId (string orderId);
    }
}

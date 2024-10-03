using TechWizWebApp.Data;

namespace TechWizWebApp.InterfaceCustomer
{
    public interface IBlogsFE
    {
        Task<CustomResult> GetAll();
        Task<CustomResult> GetById(int id);

        Task<CustomResult> GetByDesigner(int designerId);
    }
}

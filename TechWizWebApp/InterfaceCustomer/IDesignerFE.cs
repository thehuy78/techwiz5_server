using TechWizWebApp.Data;

namespace TechWizWebApp.InterfaceCustomer
{
    public interface IDesignerFE
    {
        Task<CustomResult> GetAll();

        Task<CustomResult> GetById(int id);
    }
}

using TechWizWebApp.Data;

namespace TechWizWebApp.Interface
{
    public interface IOrder
    {
        Task<CustomResult> GetAll();

        Task<CustomResult> GetById(int id);

        Task<CustomResult> ChangeStatus(string id);

        Task<CustomResult> getByMonth(DateTime month);
    }
}

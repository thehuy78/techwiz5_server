
using TechWizWebApp.Data;

namespace TechWizWebApp.Interface
{
    public interface IGallery
    {
        Task<CustomResult> GetAll();
        Task<CustomResult> GetById(int id);

        Task<CustomResult> GetByName(string name);

        Task<CustomResult> Create(TechWizWebApp.Domain.Gallery gallery);

        Task<CustomResult> Update(TechWizWebApp.Domain.Gallery gallery);

        Task<CustomResult> ChangeStatus(int id);
    }
}

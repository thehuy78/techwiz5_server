
using TechWizWebApp.Data;
using TechWizWebApp.Domain;

namespace TechWizWebApp.Interface
{
    public interface IGallery
    {
        Task<CustomResult> GetAll();
        Task<CustomResult> GetById(int id);

        Task<CustomResult> GetByName(string name);

        Task<CustomResult> Create(int userId, TechWizWebApp.Domain.Gallery gallery);

        Task<CustomResult> Update(TechWizWebApp.Domain.Gallery gallery);

        Task<CustomResult> ChangeStatus(int id);


        Task<CustomPaging> GetAdminGalleries(int pageNumber, int pageSize, bool status, List<string> by, string designerName, string colorTone, string name);

        Task<CustomPaging> GetDesignerGalleries(int userId, int pageNumber, int pageSize, bool status, string colorTone, string name);


    }
}

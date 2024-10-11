using TechWizWebApp.Data;

namespace TechWizWebApp.InterfaceCustomer
{
    public interface IGalleryFE
    {
        Task<CustomResult> GetAll();
        Task<CustomResult> GetById(int id);

        Task<CustomResult> GetByRoomType(int idRoomType);

        Task<CustomResult> GetByRoomTypeUrl(string url);

        Task<CustomResult> GetNewest(string url);
    }
}

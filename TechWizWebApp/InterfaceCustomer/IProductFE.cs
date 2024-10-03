using TechWizWebApp.Data;

namespace TechWizWebApp.InterfaceCustomer
{
    public interface IProductFE
    {
        Task<CustomResult> GetAll();

        Task<CustomResult> GetById(int id);

        Task<CustomResult> GetProductByGallery(int idGallery);


    }
}

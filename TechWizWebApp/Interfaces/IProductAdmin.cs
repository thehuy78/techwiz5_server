using Microsoft.AspNetCore.Mvc;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.RequestModels;

namespace TechWizWebApp.Interfaces
{
    public interface IProductAdmin
    {
        public Task<CustomResult> CreateNewProduct(RequestCreateNewProduct requestCreateNewProduct);

        public Task<CustomPaging> GetProductList( int pageNumber, int pageSize, bool active,  IEnumerable<int> functionalityId,  IEnumerable<string> brand, string search);

        public Task<CustomResult> GetProductSelect();

        public Task<CustomResult> ChangeProductStatus(int productId);

        public Task<CustomResult> GetSpecificProduct(ICollection<int> productId);

        public Task<CustomResult> SearchProduct(string productName);

        public Task<CustomResult> GetProduct(int productId);

        public Task<CustomResult> UpdateProduct(RequestUpdateProduct requestUpdateProduct);

    }
}

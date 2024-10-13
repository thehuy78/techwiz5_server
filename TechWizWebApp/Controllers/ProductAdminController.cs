using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechWizWebApp.Data;
using TechWizWebApp.Interfaces;
using TechWizWebApp.RequestModels;

namespace TechWizWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAdminController : ControllerBase
    {
        private IProductAdmin _productAdmin;

        public ProductAdminController(IProductAdmin productAdmin)
        {
            _productAdmin = productAdmin;
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("create_product")]
        public async Task<IActionResult> CreateProduct([FromForm] RequestCreateNewProduct requestCreateNewProduct)
        {
            var customResult = await _productAdmin.CreateNewProduct(requestCreateNewProduct);
            return Ok(customResult);
        }

        [Authorize(Roles = "admin, designer")]
        [HttpGet]
        [Route("get_products")]
        public async Task<IActionResult> GetProductList([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] bool active, [FromQuery] IEnumerable<int> functionalityId, [FromQuery] IEnumerable<string> brand, [FromQuery] string search = "")
        {
            var customPaging = await _productAdmin.GetProductList(pageNumber, pageSize, active, functionalityId, brand, search);
            return Ok(customPaging);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("GetSelect")]
        public async Task<IActionResult> GetSelect()
        {
            var customPaging = await _productAdmin.GetProductSelect();
            return Ok(customPaging);
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        [Route("change_product_status")]
        public async Task<IActionResult> ChangeProductStatus([FromForm] int productId)
        {
            var customResult = await _productAdmin.ChangeProductStatus(productId);
            return Ok(customResult);
        }

        [HttpGet]
        [Authorize]
        [Route("search_product")]
        public async Task<IActionResult> SearchProduct([FromQuery] string productName)
        {
            var customResult = await _productAdmin.SearchProduct(productName);
            return Ok(customResult);
        }

        [HttpGet]
        [Authorize]
        [Route("show_specific_product")]
        public async Task<IActionResult> GetSpecificProduct([FromQuery] ICollection<int> productId)
        {
            var customResult = await _productAdmin.GetSpecificProduct(productId);
            return Ok(customResult);
        }

        [HttpGet]
        [Authorize(Roles = "designer, admin")]
        [Route("get_product")]
        public async Task<IActionResult> GetProductById([FromQuery] int productId)
        {
            var customResult = await _productAdmin.GetProduct(productId);

            return Ok(customResult);
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        [Route("update_product")]
        public async Task<IActionResult> UpdateProduct([FromForm] RequestUpdateProduct requestUpdateProduct)
        {
            var customResult = await _productAdmin.UpdateProduct(requestUpdateProduct);

            return Ok(customResult);
        }
    }
}

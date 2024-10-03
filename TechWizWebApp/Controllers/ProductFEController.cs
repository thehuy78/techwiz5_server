using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechWizWebApp.InterfaceCustomer;

namespace TechWizWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductFEController : ControllerBase
    {
        private IProductFE _productFE;

        public ProductFEController(IProductFE iProductFE)
        {
            _productFE = iProductFE;
        }
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var result = await _productFE.GetAll();
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpGet("GetById/{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var result = await _productFE.GetById(id);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpGet("GetByGallery/{id}")]
        public async Task<ActionResult> GetByGallery(int id)
        {
            var result = await _productFE.GetProductByGallery(id);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}

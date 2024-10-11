using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechWizWebApp.InterfaceCustomer;

namespace TechWizWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsFEController : ControllerBase
    {
        private IBlogsFE _blogFE;
        public BlogsFEController(IBlogsFE blogsFE)
        {
            _blogFE = blogsFE;
        }
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var result = await _blogFE.GetAll();
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
            var result = await _blogFE.GetById(id);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpGet("GetByDesigner/{id}")]
        public async Task<ActionResult> GetByDesigner(int id)
        {
            var result = await _blogFE.GetByDesigner(id);
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

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechWizWebApp.InterfaceCustomer;

namespace TechWizWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesigneFEController : ControllerBase
    {
        private IDesignerFE _designer;
        public DesigneFEController(IDesignerFE designer)
        {
            _designer = designer;
        }
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var result = await _designer.GetAll();

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
            var result = await _designer.GetById(id);

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

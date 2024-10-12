using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechWizWebApp.InterfaceCustomer;

namespace TechWizWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationFEController : ControllerBase
    {
        private INotificationFE _repo;
        public NotificationFEController(INotificationFE repo)
        {
            _repo = repo;
        }


        [HttpGet("Get/{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var rs = await _repo.GetNotificationByUser(id);
            if(rs.Status == 200)
            {
                return Ok(rs);
            }
            else
            {
                return BadRequest(rs);
            }
        }


        [HttpGet("ReadAction/{id}")]
        public async Task<ActionResult> ReadAction(int id)
        {
            var rs = await _repo.ReadAction(id);
            if (rs.Status != 400)
            {
                return Ok(rs);
            }
            else
            {
                return BadRequest(rs);
            }
        }

        [HttpGet("Deleted/{id}")]
        public async Task<ActionResult> Deleted(int id)
        {
            var rs = await _repo.Deleted(id);
            if (rs.Status == 200)
            {
                return Ok(rs);
            }
            else
            {
                return BadRequest(rs);
            }
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechWizWebApp.InterfaceCustomer;

namespace TechWizWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypeFEController : ControllerBase
    {
        private IRoomTypeFE _roomTypeFE;
        public RoomTypeFEController(IRoomTypeFE iRoomTypeFE)
        {
            _roomTypeFE = iRoomTypeFE;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var result = await _roomTypeFE.GetAll();
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

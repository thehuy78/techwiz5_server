using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechWizWebApp.Interface;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypeController : ControllerBase
    {
        private IRoomType _roomType;
        public RoomTypeController(IRoomType roomType)
        {
            _roomType = roomType;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var result = await _roomType.GetAll();

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

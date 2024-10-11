using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using TechWizWebApp.InterfaceCustomer;
using TechWizWebApp.RepositotyCustomer;

namespace TechWizWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderFEController : ControllerBase
    {
        private IOrderFE _orderFE;

        public OrderFEController(IOrderFE orderFE)
        {
            _orderFE = orderFE;
        }
        [HttpPost("CreateOrder")]
        public async Task<ActionResult> CreateOrder([FromForm] RequestOrder request)
        {
            var result = await _orderFE.setOrder(request);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }

        }


        [HttpGet("GetOrder/{id}")]
        public async Task<ActionResult> GetOrder(int id)
        {
            var result = await _orderFE.listOrderByUser(id);
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechWizWebApp.Data;
using TechWizWebApp.InterfaceCustomer;

namespace TechWizWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailFEController : ControllerBase
    {
        private IOrderdetailFE _orderdetailFE;
        public OrderDetailFEController(IOrderdetailFE orderdetail)
        {
            _orderdetailFE = orderdetail;
        }
        [HttpGet("GetOrderDetailByOrderId/{orderId}")]
        public async Task<ActionResult> GetOrderDetailByOrderId(string orderId)
        {
            var result = await _orderdetailFE.getByOrderId(orderId);
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

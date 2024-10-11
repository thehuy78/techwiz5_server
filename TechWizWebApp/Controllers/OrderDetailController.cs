using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechWizWebApp.Data;
using TechWizWebApp.Interface;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private IOrderDetails _orderDetails;
        private readonly DecorVistaDbContext _context;
        public OrderDetailController(IOrderDetails orderDetails, DecorVistaDbContext decorVistaDbContext)
        {
            _orderDetails   = orderDetails;
            _context = decorVistaDbContext;
        }


        [HttpGet("GetByOrderId/{orderId}")]
        public async Task<ActionResult> GetByOrderId (string orderId)
        {
            var result = await _orderDetails.GetByOrderId(orderId);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        //// Nhan viet
        //[HttpGet]
        //public async Task<IActionResult> GetOrderDetailsByOrderId(string orderId)
        //{
        //    var orderDetails = await _context.OrderDetails
        //        .Include(od => od.order)
        //        .Include(od => od.variant)
        //        .Where(od => od.order_id == orderId)

        //        .ToListAsync();
        //    return Ok(new CustomResult
        //    {
        //        data = orderDetails,
        //        Message = "oke",
        //        Status = 200
        //    });
        //}
    }
}

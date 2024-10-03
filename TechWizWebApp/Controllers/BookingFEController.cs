using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechWizWebApp.Domain;
using TechWizWebApp.InterfaceCustomer;

namespace TechWizWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingFEController : ControllerBase
    {
        private IBookingFE _booking;

        public BookingFEController(IBookingFE booking)
        {
            _booking = booking;
        }
        [HttpPost("CreateBooking")]
        public async Task<ActionResult> CreateBooking([FromForm] Consultation c)
        {
            c.status = "pending";
            var result = await _booking.CreateBooking(c);

            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpGet("ListBooking/{id}")]
        public async Task<ActionResult> ListBooking(int id)
        {
            var result = await _booking.ListBookingByUserID(id);
            if(result.Status == 200)
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

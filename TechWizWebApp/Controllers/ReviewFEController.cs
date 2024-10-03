using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static TechWizWebApp.RepositotyCustomer.ReviewFERepo;
using TechWizWebApp.Domain;
using TechWizWebApp.InterfaceCustomer;

namespace TechWizWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewFEController : ControllerBase
    {
        private IReviewFE _review;
        public ReviewFEController(IReviewFE review)
        {
            _review = review;
        }

        [HttpPost("SendFeedBackConsultation")]
        public async Task<ActionResult> SendFeedBackConsultation([FromForm] Review review)
        {
            var result = await _review.SendFeedBackConsultation(review);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost("SendFeedBackProduct")]
        public async Task<ActionResult> SendFeedBackProduct([FromForm] ReviewRes review)
        {
            var result = await _review.SendFeedBackProduct(review);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpGet("GetFeedBackProduct/{id}")]
        public async Task<ActionResult> GetFeedBackProduct(int id)
        {
            var result = await _review.listReviewProduct(id);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet("GetFeedBackConsultation/{id}")]
        public async Task<ActionResult> GetFeedBackConsultation(int id)
        {
            var result = await _review.listReviewConsultationByUser(id);
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

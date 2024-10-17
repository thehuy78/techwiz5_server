using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechWizWebApp.Interfaces;

namespace TechWizWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultationAdminController : ControllerBase
    {
        private readonly IConsultationAdmin _consultationAdmin;

        public ConsultationAdminController(IConsultationAdmin consultationAdmin)
        {
            _consultationAdmin = consultationAdmin;
        }

        [Authorize(Roles = "designer")]
        [HttpGet]
        [Route("designer_consultation_list")]
        public async Task<IActionResult> GetDesignerConsultationList([FromQuery] string status,[FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string from = "", [FromQuery] string to = "", [FromQuery] string search = "")
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
            int.TryParse(idClaim, out int userId);

            var customPaging = await _consultationAdmin.GetCustomerConsultation(userId, status, pageNumber, pageSize, from, to, search);

            return Ok(customPaging);
        }


        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("consultation_list")]
        public async Task<IActionResult> GetAdminConsultationList([FromQuery] string status, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
         

            var customPaging = await _consultationAdmin.GetAdminCustomerConsultation( status, pageNumber, pageSize);

            return Ok(customPaging);
        }


        [Authorize(Roles = "designer")]
        [HttpPut]
        [Route("designer_approve_consultation")]
        public async Task<IActionResult> DesignerApproveConsultation([FromForm] int consultationId)
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
            int.TryParse(idClaim, out int userId);

            var customResult = await _consultationAdmin.AcceptConsultation(userId, consultationId);

            return Ok(customResult);
        }

        [Authorize(Roles = "designer")]
        [HttpPut]
        [Route("designer_finished_consultation")]
        public async Task<IActionResult> DesignerFinishConsultation([FromForm] int consultationId)
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
            int.TryParse(idClaim, out int userId);

            var customResult = await _consultationAdmin.FinishedConsultation(userId, consultationId);

            return Ok(customResult);
        }


        [Authorize(Roles = "designer")]
        [HttpPut]
        [Route("designer_deny_consultation")]
        public async Task<IActionResult> DesignerDenyConsultation([FromForm] int consultationId)
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
            int.TryParse(idClaim, out int userId);

            var customResult = await _consultationAdmin.DenyConsultation(userId, consultationId);

            return Ok(customResult);
        }

    }
}

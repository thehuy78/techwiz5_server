using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using TechWizWebApp.Data;
using TechWizWebApp.Interfaces;
using TechWizWebApp.RequestModels;
using TechWizWebApp.Services;

namespace TechWizWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignerAdminController : ControllerBase
    {
        private readonly IDesignerAdmin _designerAdmin;
        private readonly DecorVistaDbContext _context;
        private readonly IFileService _fileService;
        private readonly IMailService _mailService;

        public DesignerAdminController(IDesignerAdmin designerAdmin, DecorVistaDbContext context, IFileService fileService, IMailService mailService)
        {
            _designerAdmin = designerAdmin;
            _context = context;
            _fileService = fileService;
            _mailService = mailService;
        }

        [HttpPost]
        [Route("designer_register")]
        public async Task<IActionResult> DesignerRegister(RequestDesignerRegister requestDesignerRegister)
        {
            var customResult = await _designerAdmin.DesignerRegister(requestDesignerRegister);

            return Ok(customResult);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("pending_list")]
        public async Task<IActionResult> GetPendingApprovedDesigner([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] int year, [FromQuery] string specialize = "", [FromQuery] string search = "")
        {
            var customPaging = await _designerAdmin.GetListPendingDesigner(pageNumber, pageSize, year, specialize, search);

            return Ok(customPaging);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("approved_list")]
        public async Task<IActionResult> GetApprovedDesigner([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] int year, [FromQuery] bool status, [FromQuery] string specialize = "", [FromQuery] string search = "")
        {
            var customPaging = await _designerAdmin.GetListApprovedDesigner(pageNumber, pageSize, year, status, specialize, search);

            return Ok(customPaging);
        }

        [Authorize(Roles = "admin, designer")]
        [HttpGet]
        [Route("get_designer")]
        public async Task<IActionResult> GetDesignerById([FromQuery] int designerId)
        {
            var customResult = await _designerAdmin.GetDesignerById(designerId);

            return Ok(customResult);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("get_approved_designer")]
        public async Task<IActionResult> GetApprovedDesignerById([FromQuery] int designerId)
        {
            var customResult = await _designerAdmin.GetApproveDesignerById(designerId);

            return Ok(customResult);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("get_unapproved_designer")]
        public async Task<IActionResult> GetUnapprovedDesignerById([FromQuery] int designerId)
        {
            var customResult = await _designerAdmin.GetUnapproveDesignerById(designerId);

            return Ok(customResult);
        }

        [Authorize(Roles = "designer")]
        [HttpGet]
        [Route("designer_profile")]
        public async Task<IActionResult> GetProfile()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
            int.TryParse(idClaim, out int userId);

            var customResult = await _designerAdmin.GetDesignerById(userId);

            return Ok(customResult);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("approve_designer")]
        public async Task<IActionResult> ApproveDesigner([FromForm] int designerId)
        {
            var customResult = await _designerAdmin.ApproveDesigner(designerId);

            return Ok(customResult);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("deny_designer")]
        public async Task<IActionResult> DenyDesigner([FromForm] int designerId)
        {
            var customResult = await _designerAdmin.DenyDesigner(designerId);

            return Ok(customResult);
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("change_status")]
        public async Task<IActionResult> ChangeDesignerStatus([FromForm] int designerId)
        {
            var customResult = await _designerAdmin.ChangeDesignerStatus(designerId);

            return Ok(customResult);
        }

        [Authorize(Roles = "designer")]
        [HttpPut]
        [Route("update_info")]
        public async Task<IActionResult> ChangeDesignerInfo([FromForm] RequestUpdateDesignerInfo requestUpdateDesignerInfo)
        {
            var customResult = await _designerAdmin.ChangeDesignerInfo(requestUpdateDesignerInfo);

            return Ok(customResult);
        }

        [Authorize(Roles = "designer")]
        [HttpPut]
        [Route("update_dow")]
        public async Task<IActionResult> ChangeDow([FromForm] string dow)
        {

            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
            int.TryParse(idClaim, out int userId);

            var customResult = await _designerAdmin.ChangeDow(userId, dow);

            return Ok(customResult);
        }

        [Authorize(Roles = "designer")]
        [HttpPut]
        [Route("update_portfolio")]
        public async Task<IActionResult> ChangePortfolio([FromForm] string portfolio)
        {

            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
            int.TryParse(idClaim, out int userId);

            var customResult = await _designerAdmin.ChangePortfolio(userId, portfolio);

            return Ok(customResult);
        }

        [Authorize(Roles = "designer")]
        [HttpPut]
        [Route("update_certificate")]
        public async Task<IActionResult> ChangePortfolio([FromForm] UpdateCertificate updateCertificate)
        {
            var customResult = await _designerAdmin.UpdateCertificate(updateCertificate);

            return Ok(customResult);
        }



        [Authorize(Roles = "designer")]
        [HttpPut]
        [Route("update_image")]
        public async Task<IActionResult> ChangeAvatar([FromForm] IFormFile avatar)
        {

            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
            int.TryParse(idClaim, out int userId);

            var customResult = await _designerAdmin.UpdateImage(userId, avatar);

            return Ok(customResult);
        }

        [HttpPut("ForgortPassword")]
        public async Task<ActionResult> ForgotPassword([FromForm] ForgotPasswordRequest request)
        {
            //kiem tra email da duoc tao chua
            var user = await _context.Users.Include(u => u.userdetails).Where(u => u.email == request.email).FirstOrDefaultAsync();
            if (user == null)
            {
                return Ok(new CustomResult
                {
                    data = null,
                    Message = "Email not found",
                    Status = 400
                });
            }

            //Gui mail

            //encond userId
            string encodedUserId = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(user.id.ToString()));
            string frontendUrl = $"http://localhost:5173/forgotPassword?id={encodedUserId}";

            string imgUrl = "<img src=\"https://firebasestorage.googleapis.com/v0/b/techwizwebapp.appspot.com/o/Images%2Fef8273f5-d9bf-4b95-8602-5f1de021201a.png?alt=media&token=c9257fe5-ce0a-46d3-b541-a1333d0c3f58\" alt=\"Image\" width=\"200\" height=\"160\" />";
            string resetPasswordUrl = $"https://localhost:7229/api/AuthUser/ResetPassword?userId={user.id}"; // lay cai nay
            string emailContent = @$"
                Dear {user?.userdetails?.first_name} {user?.userdetails?.last_name},  <div></div>

                We have received a request to reset the password for your account at DecorVista.   <div></div>

                To reset your password, please click the link below:  <div></div>

                <a href=""{frontendUrl}"">Click here</a> <div></div>

                Best regards,
                <div>{imgUrl}</div>

                ";
            await _mailService.SendMailAsync(user?.email, "Forgot Password Information", emailContent);


            return Ok(new CustomResult
            {
                data = null,
                Message = "Oke",
                Status = 200
            });
        }


        public class UpdateCertificate
        {
            public int DesignerId { get; set; }
            public ICollection<string>? OldList { get; set; }

            public ICollection<IFormFile>? NewImages { get; set; }

        }

        public class ForgotPasswordRequest
        {
            public string? email { get; set; }
        }

        [HttpGet("ResetPassword")]
        public async Task<ActionResult> ResetPassword([FromQuery] int userId)
        {
            var user = await _context.Users.Include(u => u.userdetails).FirstOrDefaultAsync(u => u.id == userId);

            var passwordReset = Guid.NewGuid().ToString();
            user.password = BCrypt.Net.BCrypt.HashPassword(passwordReset);
            _context.Update(user);
            await _context.SaveChangesAsync();


            string imgUrl = "<img src=\"https://firebasestorage.googleapis.com/v0/b/techwizwebapp.appspot.com/o/Images%2Fef8273f5-d9bf-4b95-8602-5f1de021201a.png?alt=media&token=c9257fe5-ce0a-46d3-b541-a1333d0c3f58\" alt=\"Image\" width=\"200\" height=\"160\" />";
            string emailContent = @$"
Dear {user?.userdetails?.first_name} {user?.userdetails?.last_name}, <div></div>
Your new Password is: {passwordReset}   <div></div>
Please update your password soon    <div></div>
<div>{imgUrl}</div>
";
            await _mailService.SendMailAsync(user?.email, "New Password Information", emailContent);


            return Ok(new CustomResult
            {
                Status = 200,
                data = null,
                Message = "Oke"
            });

        }
    }
}

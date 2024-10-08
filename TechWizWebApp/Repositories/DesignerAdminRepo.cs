using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Cms;
using System.Numerics;
using TechWizWebApp.Controllers;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.Interfaces;
using TechWizWebApp.RequestModels;
using TechWizWebApp.Services;

namespace TechWizWebApp.Repositories
{
    public class DesignerAdminRepo : IDesignerAdmin
    {
        private readonly DecorVistaDbContext _context;
        private readonly IConfiguration _config;
        private readonly IFileService _fileService;
        private readonly IMailService _mailService;

        public DesignerAdminRepo(DecorVistaDbContext context, IConfiguration config, IFileService fileService, IMailService mailService)
        {
            _context = context;
            _config = config;
            _fileService = fileService;
            _mailService = mailService;
        }

        public async Task<CustomResult> DesignerRegister(RequestDesignerRegister requestDesignerRegister)
        {
            try
            {
                var isEmailOk = await CheckEmailExist(requestDesignerRegister.Email);

                if (!isEmailOk)
                {
                    return new CustomResult(403, "Email already exist", null);
                }

                var isPhoneOk = await CheckPhoneNumberExist(requestDesignerRegister.Phone);

                if (!isPhoneOk)
                {
                    return new CustomResult(403, "Phone number already exist", null);
                }

                var newUser = new User
                {
                    email = requestDesignerRegister.Email,
                    password = BCrypt.Net.BCrypt.HashPassword(requestDesignerRegister.Password),
                };

                _context.Users.Add(newUser);

                var newDesigner = new InteriorDesigner
                {
                    address = requestDesignerRegister.Address,
                    approved_status = "pending",
                    portfolio = requestDesignerRegister.Porfolio,
                    contact_number = requestDesignerRegister.Phone,
                    user = newUser,
                    first_name = requestDesignerRegister.FirstName,
                    last_name = requestDesignerRegister.LastName,
                    specialization = requestDesignerRegister.Specialization,
                    yearsofexperience = requestDesignerRegister.Year,
                    status = true,
                };


                if (requestDesignerRegister.Certificate != null || requestDesignerRegister.Certificate.Count != 0)
                {
                    string certificateString = "";
                    foreach (var image in requestDesignerRegister.Certificate)
                    {
                        var imageName = await _fileService.UploadImageAsync(image);
                        certificateString = certificateString + "; " + imageName;
                    }
                    newDesigner.certificate = certificateString;

                }

                if (requestDesignerRegister.Avatar != null)
                {
                    var avatar = await _fileService.UploadImageAsync(requestDesignerRegister.Avatar);
                    newDesigner.avatar = avatar;
                }

                _context.InteriorDesigners.Add(newDesigner);
                await _context.SaveChangesAsync();


                var admins = await _context.UserDetails.Where(u => u.role == "admin").ToListAsync();

                foreach(var admin in admins)
                {
                    var newNotification = new Notification
                    {
                        created_date = DateTime.Now,
                        is_read = false,
                        message = $@"New designer with an name {newDesigner.first_name + " " + newDesigner.last_name} has successfully registered on DecorVista",
                        type = "admin:designer",
                        url = "/pending_detail?id=" + newDesigner.user_id,
                        user_id = admin.user_id
                    };

                    _context.Notifications.Add(newNotification);
                }

                await _context.SaveChangesAsync();

                return new CustomResult(200, "Ok", newUser);

            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Bad request", ex.Message);
            }
        }

        public async Task<bool> CheckEmailExist(string email)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.email == email);

                if (user == null)
                {
                    return true;
                }

                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CheckPhoneNumberExist(string phone)
        {
            try
            {
                var user = await _context.InteriorDesigners.SingleOrDefaultAsync(u => u.contact_number == phone);

                if (user == null)
                {
                    return true;
                }

                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<CustomResult> GetDesignerById(int id)
        {
            try
            {
                var designer = await _context.InteriorDesigners.Include(d => d.user).SingleOrDefaultAsync(d => d.user_id == id);

                if (designer == null)
                {
                    return new CustomResult(404, "Not found", null);
                }

                return new CustomResult(200, "Success", designer);

            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Bad request", ex.Message);
            }
        }

        public async Task<CustomResult> GetUnapproveDesignerById(int id)
        {
            try
            {
                var designer = await _context.InteriorDesigners.Include(d => d.user).SingleOrDefaultAsync(d => d.user_id == id && d.approved_status == "pending");

                if (designer == null)
                {
                    return new CustomResult(404, "Not found", null);
                }

                return new CustomResult(200, "Success", designer);

            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Bad request", ex.Message);
            }
        }

        public async Task<CustomResult> GetApproveDesignerById(int id)
        {
            try
            {
                var designer = await _context.InteriorDesigners.Include(d => d.user).SingleOrDefaultAsync(d => d.user_id == id && d.approved_status == "approved");

                if (designer == null)
                {
                    return new CustomResult(404, "Not found", null);
                }

                return new CustomResult(200, "Success", designer);

            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Bad request", ex.Message);
            }
        }

        public async Task<CustomPaging> GetListPendingDesigner(int pageNumber, int pageSize, int year, string specialize, string search)
        {
            try
            {

                var designers = _context.InteriorDesigners.OrderByDescending(p => p.id).Include(d => d.user).Where(d => d.approved_status == "pending" && (d.first_name.Contains(search) || d.last_name.Contains(search) || d.address.Contains(search) || d.user.email.Contains(search)) && (d.specialization.Contains(specialize)) && d.yearsofexperience >= year);


                var total = designers.Count();

                designers = designers.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);


                var list = await designers.ToListAsync();

                var customPaging = new CustomPaging()
                {
                    Status = 200,
                    Message = "OK",
                    CurrentPage = pageNumber,
                    TotalPages = (int)Math.Ceiling((double)total / pageSize),
                    PageSize = pageSize,
                    TotalCount = total,
                    Data = list
                };

                return customPaging;

            }
            catch (Exception ex)
            {
                return new CustomPaging()
                {
                    Status = 400,
                    Message = ex.Message,
                    CurrentPage = pageNumber,
                    TotalPages = 0,
                    PageSize = pageSize,
                    TotalCount = 0,
                    Data = null
                };
            }
        }

        public async Task<CustomPaging> GetListApprovedDesigner(int pageNumber, int pageSize, int year, bool status, string specialize, string search)
        {
            try
            {

                IQueryable<InteriorDesigner> query;

                query = _context.InteriorDesigners;

                query = query.OrderByDescending(p => p.id).Include(d => d.user).Where(d => d.approved_status == "approved" && (d.first_name.Contains(search) || d.last_name.Contains(search) || d.address.Contains(search) || d.user.email.Contains(search)) && (d.specialization.Contains(specialize)) && d.yearsofexperience >= year && d.status == status);

                var total = query.Count();

                query = query.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);


                var list = await query.ToListAsync();

                var customPaging = new CustomPaging()
                {
                    Status = 200,
                    Message = "OK",
                    CurrentPage = pageNumber,
                    TotalPages = (int)Math.Ceiling((double)total / pageSize),
                    PageSize = pageSize,
                    TotalCount = total,
                    Data = list
                };

                return customPaging;

            }
            catch (Exception ex)
            {
                return new CustomPaging()
                {
                    Status = 400,
                    Message = ex.Message,
                    CurrentPage = pageNumber,
                    TotalPages = 0,
                    PageSize = pageSize,
                    TotalCount = 0,
                    Data = null
                };
            }
        }

        public async Task<CustomResult> ApproveDesigner(int designerId)
        {
            try
            {
                var designer = await _context.InteriorDesigners.Include(d => d.user).SingleOrDefaultAsync(d => d.id == designerId);

                _mailService.SendMailAsync(designer.user.email, "Accept as Designer", approveDesignerBody(designer.user.email));

                if (designer == null)
                {
                    return new CustomResult(404, "Not found", null);
                }

                designer.approved_status = "approved";

                _context.InteriorDesigners.Update(designer);

                await _context.SaveChangesAsync();

                return new CustomResult(200, "Success", designer);


            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Bad request", ex.Message);
            }
        }

        public async Task<CustomResult> DenyDesigner(int designerId)
        {
            try
            {
                var designer = await _context.InteriorDesigners.Include(d => d.user).SingleOrDefaultAsync(d => d.id == designerId);

                _mailService.SendMailAsync(designer.user.email, "Deny designer", denyDesigner());

                if (designer == null)
                {
                    return new CustomResult(404, "Not found", null);
                }

                designer.approved_status = "unapproved";

                _context.InteriorDesigners.Update(designer);

                await _context.SaveChangesAsync();

                return new CustomResult(200, "Success", designer);


            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Bad request", ex.Message);
            }
        }

        public async Task<CustomResult> ChangeDesignerStatus(int designerId)
        {
            try
            {
                var designer = await _context.InteriorDesigners.Include(d => d.user).SingleOrDefaultAsync(d => d.id == designerId);

                if (designer == null)
                {
                    return new CustomResult(404, "Not found", null);
                }

                designer.status = !designer.status;

                _context.InteriorDesigners.Update(designer);

                await _context.SaveChangesAsync();

                return new CustomResult(200, "Success", designer);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Bad request", ex.Message);
            }
        }

        public async Task<CustomResult> ChangeDow(int designerId, string dow)
        {
            try
            {
                var designer = await _context.InteriorDesigners.Include(d => d.user).SingleOrDefaultAsync(d => d.user_id == designerId);

                if (designer == null)
                {
                    return new CustomResult(404, "Not found", null);
                }

                designer.daywork = dow;

                _context.InteriorDesigners.Update(designer);

                await _context.SaveChangesAsync();

                var admins = await _context.UserDetails.Where(u => u.role == "admin").ToListAsync();

                foreach (var admin in admins)
                {
                    var newNotification = new Notification
                    {
                        created_date = DateTime.Now,
                        is_read = false,
                        message = $@"Designer with an name {designer.first_name + " " + designer.last_name} has changed their day of work ",
                        type = "admin:designer",
                        url = "/designer_detail?id=" + designer.user_id,
                        user_id = admin.user_id
                    };

                    _context.Notifications.Add(newNotification);
                }

                await _context.SaveChangesAsync();

                return new CustomResult(200, "Success", designer);

            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Bad request", ex.Message);
            }
        }

        public async Task<CustomResult> ChangePortfolio(int designerId, string portfolio)
        {
            try
            {
                var designer = await _context.InteriorDesigners.Include(d => d.user).SingleOrDefaultAsync(d => d.user_id == designerId);

                if (designer == null)
                {
                    return new CustomResult(404, "Not found", null);
                }

                designer.portfolio = portfolio;

                _context.InteriorDesigners.Update(designer);

                await _context.SaveChangesAsync();

                var admins = await _context.UserDetails.Where(u => u.role == "admin").ToListAsync();

                foreach (var admin in admins)
                {
                    var newNotification = new Notification
                    {
                        created_date = DateTime.Now,
                        is_read = false,
                        message = $@"Designer with an name {designer.first_name + " " + designer.last_name} has changed their portfolio ",
                        type = "admin:designer",
                        url = "/designer_detail?id=" + designer.user_id,
                        user_id = admin.user_id
                    };

                    _context.Notifications.Add(newNotification);
                }

                await _context.SaveChangesAsync();

                return new CustomResult(200, "Success", designer);

            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Bad request", ex.Message);
            }
        }

        public async Task<CustomResult> ChangeDesignerInfo(RequestUpdateDesignerInfo requestUpdateDesignerInfo)
        {
            try
            {
                var designer = await _context.InteriorDesigners.Include(d => d.user).SingleOrDefaultAsync(d => d.id == requestUpdateDesignerInfo.Id);

                if (designer == null)
                {
                    return new CustomResult(404, "Not found", null);
                }

                designer.first_name = requestUpdateDesignerInfo.FirstName;
                designer.last_name = requestUpdateDesignerInfo.LastName;
                designer.contact_number = requestUpdateDesignerInfo.Phone;
                designer.address = requestUpdateDesignerInfo.Address;
                designer.specialization = requestUpdateDesignerInfo.Specialization;
                designer.yearsofexperience = requestUpdateDesignerInfo.Year;

                _context.InteriorDesigners.Update(designer);

                await _context.SaveChangesAsync();

                var admins = await _context.UserDetails.Where(u => u.role == "admin").ToListAsync();

                foreach (var admin in admins)
                {
                    var newNotification = new Notification
                    {
                        created_date = DateTime.Now,
                        is_read = false,
                        message = $@"Designer with an name {designer.first_name + " " + designer.last_name} has changed their info ",
                        type = "admin:designer",
                        url = "/designer_detail?id=" + designer.user_id,
                        user_id = admin.user_id
                    };

                    _context.Notifications.Add(newNotification);
                }

                await _context.SaveChangesAsync();

                return new CustomResult(200, "Success", designer);

            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Bad request", ex.Message);
            }
        }

        public async Task<CustomResult> UpdateImage(int designerId, IFormFile avatar)
        {
            try
            {
                var designer = await _context.InteriorDesigners.Include(d => d.user).SingleOrDefaultAsync(d => d.user_id == designerId);

                if (designer == null)
                {
                    return new CustomResult(404, "Not found", null);
                }

                var newAvatar = await _fileService.UploadImageAsync(avatar);

                designer.avatar = newAvatar;

                _context.InteriorDesigners.Update(designer);

                await _context.SaveChangesAsync();

                var admins = await _context.UserDetails.Where(u => u.role == "admin").ToListAsync();

                foreach (var admin in admins)
                {
                    var newNotification = new Notification
                    {
                        created_date = DateTime.Now,
                        is_read = false,
                        message = $@"Designer with an name {designer.first_name + " " + designer.last_name} has changed their avatar ",
                        type = "admin:designer",
                        url = "/designer_detail?id=" + designer.user_id,
                        user_id = admin.user_id
                    };

                    _context.Notifications.Add(newNotification);
                }

                await _context.SaveChangesAsync();

                return new CustomResult(200, "Success", designer);

            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Bad request", ex.Message);
            }
        }

        public async Task<CustomResult> UpdateCertificate(DesignerAdminController.UpdateCertificate updateCertificate)
        {
            try
            {
                var designer = await _context.InteriorDesigners.SingleOrDefaultAsync(d => d.id == updateCertificate.DesignerId);

                if (designer == null)
                {
                    return new CustomResult(404, "Not found", null);
                }

                string certificateString = "";

                if (updateCertificate.OldList != null && updateCertificate.OldList.Count != 0)
                {
                    foreach (var image in updateCertificate.OldList)
                    {
                        certificateString = certificateString + "; " + image;
                    }
                }

                if (updateCertificate.NewImages != null && updateCertificate.NewImages.Count != 0)
                {

                    foreach (var image in updateCertificate.NewImages)
                    {
                        var imageName = await _fileService.UploadImageAsync(image);
                        certificateString = certificateString + "; " + imageName;
                    }
                }

                designer.certificate = certificateString;

                _context.InteriorDesigners.Update(designer);

                await _context.SaveChangesAsync();

                var admins = await _context.UserDetails.Where(u => u.role == "admin").ToListAsync();

                foreach (var admin in admins)
                {
                    var newNotification = new Notification
                    {
                        created_date = DateTime.Now,
                        is_read = false,
                        message = $@"Designer with an name {designer.first_name + " " + designer.last_name} has changed their certification ",
                        type = "admin:designer",
                        url = "/designer_detail?id=" + designer.user_id,
                        user_id = admin.user_id
                    };

                    _context.Notifications.Add(newNotification);
                }

                await _context.SaveChangesAsync();

                return new CustomResult(200, "Success", null);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Bad request", ex.Message);
            }
        }

        public string approveDesignerBody(string email)
        {
            string emailContent = $@"
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            color: #333;
            padding: 20px;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            background-color: #fff;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            background-color: #4CAF50;
            color: white;
            padding: 10px;
            text-align: center;
            border-radius: 10px 10px 0 0;
        }}
        .content {{
            margin: 20px 0;
        }}
        .content h1 {{
            font-size: 24px;
            color: #333;
        }}
        .content p {{
            font-size: 16px;
            line-height: 1.6;
        }}
        .footer {{
            margin-top: 30px;
            font-size: 14px;
            color: #777;
            text-align: center;
        }}
        .footer a {{
            color: #4CAF50;
            text-decoration: none;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>Welcome to DecorVista!</h2>
        </div>
        <div class='content'>
            <h1>Congratulations, You are now a Verified Designer!</h1>
            <p>Dear Designer,</p>
            <p>
                We are thrilled to inform you that your profile has been successfully verified by our admin team. 
                You can now start showcasing your interior design portfolio, interact with clients, and manage your consultations.
            </p>
            <p>Below are your login details to get started:</p>
            <ul>
                <li><strong>Email:</strong> {email}</li>
            </ul>
            <p>
                Please log in to your account and start exploring the platform.
            </p>
            <p>
                If you have any questions, feel free to <a href='#'>contact our support team</a>.
            </p>
        </div>
        <div class='footer'>
            <p>Thank you for choosing DecorVista!</p>
            <p>&copy; 2024 DecorVista | All Rights Reserved</p>
        </div>
    </div>
</body>
</html>";

            return emailContent;
        }

        public string denyDesigner()
        {
            string denyEmailContent = $@"
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            color: #333;
            padding: 20px;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            background-color: #fff;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            background-color: #FF6347;
            color: white;
            padding: 10px;
            text-align: center;
            border-radius: 10px 10px 0 0;
        }}
        .content {{
            margin: 20px 0;
        }}
        .content h1 {{
            font-size: 24px;
            color: #333;
        }}
        .content p {{
            font-size: 16px;
            line-height: 1.6;
        }}
        .footer {{
            margin-top: 30px;
            font-size: 14px;
            color: #777;
            text-align: center;
        }}
        .footer a {{
            color: #FF6347;
            text-decoration: none;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>Application Status: Denied</h2>
        </div>
        <div class='content'>
            <h1>We're Sorry, Your Designer Application Was Not Approved</h1>
            <p>Dear Designer,</p>
            <p>
                After careful review, we regret to inform you that your application to join DecorVista as a verified designer has not been approved at this time. 
            </p>
            <p>
                We appreciate your interest in our platform and encourage you to enhance your portfolio or provide additional information if you wish to reapply in the future.
            </p>
            <p>If you have any questions or need further details, feel free to <a href='#'>contact our support team</a>.</p>
        </div>
        <div class='footer'>
            <p>Thank you for considering DecorVista!</p>
            <p>&copy; 2024 DecorVista | All Rights Reserved</p>
        </div>
    </div>
</body>
</html>";

            return denyEmailContent;

        }


    }
}

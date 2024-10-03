using Microsoft.AspNetCore.Mvc;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.RequestModels;
using static TechWizWebApp.Controllers.DesignerAdminController;

namespace TechWizWebApp.Interfaces
{
    public interface IDesignerAdmin
    {
        Task<CustomResult> DesignerRegister(RequestDesignerRegister requestDesignerRegister);

        Task<CustomResult> GetDesignerById(int id);

        Task<CustomResult> GetUnapproveDesignerById(int id);

        Task<CustomResult> GetApproveDesignerById(int id);

        Task<CustomPaging> GetListPendingDesigner(int pageNumber, int pageSize, int year, string specialize, string search);

        Task<CustomPaging> GetListApprovedDesigner(int pageNumber, int pageSize, int year, bool status, string specialize, string search);

        Task<CustomResult> ApproveDesigner(int designerId);

        Task<CustomResult> DenyDesigner(int designerId);

        Task<CustomResult> ChangeDesignerStatus(int designerId);

        Task<CustomResult> ChangeDow(int designerId, string dow);

        Task<CustomResult> ChangePortfolio(int designerId, string portfolio);

        Task<CustomResult> ChangeDesignerInfo(RequestUpdateDesignerInfo requestUpdateDesignerInfo);

        Task<CustomResult> UpdateImage(int designerId, IFormFile avatar);



        Task<CustomResult> UpdateCertificate(UpdateCertificate updateCertificate);
    }
}

using Microsoft.AspNetCore.Mvc;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.RequestModels;

namespace TechWizWebApp.Interfaces
{
    public interface IDesignerAdmin
    {
        Task<CustomResult> DesignerRegister(RequestDesignerRegister requestDesignerRegister);

        Task<CustomResult> GetDesignerById(int id);

        Task<CustomPaging> GetListPendingDesigner(int pageNumber, int pageSize, string search); 

        Task<CustomPaging> GetListApprovedDesigner(int pageNumber, int pageSize, string search);

        Task<CustomResult> ApproveDesigner(int designerId);

        Task<CustomResult> DenyDesigner(int designerId);

        Task<CustomResult> ChangeDesignerStatus(int designerId);

        Task<CustomResult> ChangeDow(int designerId, string dow);

        Task<CustomResult> ChangePortfolio(int designerId, string portfolio);

        Task<CustomResult> ChangeDesignerInfo(RequestUpdateDesignerInfo requestUpdateDesignerInfo);

        Task<CustomResult> UpdateImage(int designerId, IFormFile avatar);


    }
}

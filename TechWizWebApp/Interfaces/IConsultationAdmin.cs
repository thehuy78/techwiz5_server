using TechWizWebApp.Data;
using TechWizWebApp.Domain;

namespace TechWizWebApp.Interfaces
{
    public interface IConsultationAdmin
    {
        Task<CustomPaging> GetCustomerConsultation(int designerId, string status, int pageNumber, int pageSize);
        Task<CustomResult> AcceptConsultation(int designerId, int consultationId);
        Task<CustomResult> DenyConsultation(int designerId, int consultationId);

        Task<CustomPaging> GetAdminCustomerConsultation(string status, int pageNumber, int pageSize);
    }
}

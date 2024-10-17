using TechWizWebApp.Data;
using TechWizWebApp.Domain;

namespace TechWizWebApp.Interfaces
{
    public interface IConsultationAdmin
    {
        Task<CustomPaging> GetCustomerConsultation(int designerId, string status, int pageNumber, int pageSize, string from, string to, string search);
        Task<CustomResult> AcceptConsultation(int designerId, int consultationId);
        Task<CustomResult> DenyConsultation(int designerId, int consultationId);

        Task<CustomResult> FinishedConsultation(int designerId, int consultationId);

        Task<CustomPaging> GetAdminCustomerConsultation(string status, int pageNumber, int pageSize);
    }
}

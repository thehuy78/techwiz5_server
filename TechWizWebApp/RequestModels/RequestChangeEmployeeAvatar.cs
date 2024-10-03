using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TechWizWebApp.RequestModels
{
    public class RequestChangeEmployeeAvatar
    {
        public int EmployeeId { get; set; }

        public IFormFile Avatar { get; set; }
    }
}

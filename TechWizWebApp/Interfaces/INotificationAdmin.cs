using TechWizWebApp.Data;
using TechWizWebApp.Domain;

namespace TechWizWebApp.Interfaces
{
    public interface INotificationAdmin
    {
        public Task<CustomPaging> GetAdminNotification(int userId, string type, int pageNumber, int pageSize);
        public Task<CustomResult> DeleteNotification(int notificationId);
        public Task<CustomResult> MarkAsReadNotification(int notificationId);

        public Task<CustomResult> SendNotification(ICollection<string> sendTo, string message);
    }
}

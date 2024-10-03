using Microsoft.Identity.Client;

namespace TechWizWebApp.RequestModels
{
    public class RequestSaveMessage
    {
        public int Id { get; set; }

        public string Message { get; set; }
    }
}

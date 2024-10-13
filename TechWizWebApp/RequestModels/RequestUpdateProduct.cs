using TechWizWebApp.Domain;

namespace TechWizWebApp.RequestModels
{
    public class RequestUpdateProduct
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int RoomFuncion { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Status { get; set; }
        public ICollection<string>? OldImages {  get; set; }
        public ICollection<IFormFile>? UploadImages { get; set; }
       
        public ICollection<string> VariantJson { get; set; }
     

    }
}

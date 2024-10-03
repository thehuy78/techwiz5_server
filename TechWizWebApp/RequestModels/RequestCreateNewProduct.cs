using TechWizWebApp.Domain;

namespace TechWizWebApp.RequestModels
{
    public class RequestCreateNewProduct
    {
        public string ProductName { get; set; } = string.Empty;
        public int RoomFuncion { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Status { get; set; }
        public ICollection<IFormFile> Images { get; set; }
        public ICollection<string> Variants { get; set; }
        public ICollection<string> VariantsJSON { get; set; }

        public ICollection<VariantDetail> VariantDetails { get; set; } = new List<VariantDetail>();

        public ICollection<string>? colorJson { get; set; }
    }
}

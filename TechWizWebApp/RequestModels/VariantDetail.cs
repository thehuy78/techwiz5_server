namespace TechWizWebApp.RequestModels
{
    public class VariantDetail
    {
        public ICollection<string> variant { get; set; }

        public float RealPrice { get; set; }

        public float FakePrice { get; set; }
    }
}

using Org.BouncyCastle.Asn1;

namespace TechWizWebApp.RequestModels
{
    public class RequestDesignerRegister
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string Specialization { get; set; }

        public string Year { get; set; }

        public string? Porfolio {  get; set; }

        public IFormFile? Avatar { get; set; }

    }
}

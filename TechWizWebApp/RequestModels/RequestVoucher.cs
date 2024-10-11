using System.Security.Principal;


namespace TechWizWebApp.RequestModels
{
    public class RequestVoucher
    {
        public int Id { get; set; }

        public string UniqueCode { get; set; }

        public ICollection<int> RankIds { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public int SalePercent { get; set; }

        public int Amount { get; set; }

        public int MimimumAvailable { get; set; }
    }
}

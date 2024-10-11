namespace TechWizWebApp.RequestModels
{
    public class RequestUpdateRank { 
    
        public int Id { get; set; }

        public string Name { get; set; }

        public int MinimumSpending { get; set; }

        public int MaximumSpending { get; set; }
    }
}

namespace TechWizWebApp.Data
{
    public class CustomResult
    {
        public int Status { get; set; }
        public string? Message { get; set; }
        public object? data { get; set; }

        public CustomResult(int status, string message, object data)
        {
            Status = status;
            Message = message;
            this.data = data;
        }
        public CustomResult()
        {

        }

    }
}




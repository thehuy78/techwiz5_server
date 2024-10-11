namespace TechWizWebApp.RequestModels
{
    public class RequestChatMessage
    {
        public int Id { get; set; }

        public int RoomId { get; set; }

        public string Email { get; set; }

        public string Message { get; set; }

        public string Avatar { get; set; }


    }
}

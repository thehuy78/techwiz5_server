using Microsoft.AspNetCore.SignalR;
using TechWizWebApp.RequestModels;

namespace TechWizWebApp.Hubs
{
    public class ChatHub : Hub
    {

        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        public async Task JoinChatRoom(RequestJoinChatRoom requestJoinChatRoom)
        {
            if (requestJoinChatRoom == null)
            {
                _logger.LogError("ChatConnection is null.");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId,"chat" + requestJoinChatRoom.Id.ToString());

            _logger.LogInformation($"{requestJoinChatRoom.Email} joined room {"chat" + requestJoinChatRoom.Id.ToString()}");
        }

        public async Task LeaveChatRoom(RequestJoinChatRoom requestJoinChatRoom)
        {
            if (requestJoinChatRoom == null)
            {
                _logger.LogError("ChatConnection is null.");
                return;
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "chat" + requestJoinChatRoom.Id.ToString());

            _logger.LogInformation($"{requestJoinChatRoom.Email} joined room {"chat" + requestJoinChatRoom.Id.ToString()}");
        }

        public async Task JoinAdminRoom(RequestJoinChatRoom requestJoinChatRoom)
        {
            if (requestJoinChatRoom == null)
            {
                _logger.LogError("ChatConnection is null.");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, "admin_chat");

            _logger.LogInformation($"{requestJoinChatRoom.Email} joined room {requestJoinChatRoom.Id.ToString()}");
        }

        public async Task UserSendMessageAdmin(RequestChatMessage requestChatMessage)
        {
            if (requestChatMessage == null)
            {
                _logger.LogError("ChatConnection is null.");
                return;
            }

            await Clients.Group("chat" + requestChatMessage.Id.ToString()).SendAsync("ReceiveUserRoomMessage", requestChatMessage);
            await Clients.Group("admin_chat").SendAsync("NotifyAdmin", requestChatMessage);
        }

        public async Task AdminSendMessageUser(RequestChatMessage requestChatMessage)
        {
            if (requestChatMessage == null)
            {
                _logger.LogError("ChatConnection is null.");
                return;
            }

            await Clients.Group("chat" + requestChatMessage.RoomId.ToString()).SendAsync("ReceiveAdminMessage", requestChatMessage);
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using HrApi.Models;
namespace HrBackend.Hubs
{
    public class ChatHub : Hub
    {
        private readonly HrContext _context;

        public ChatHub(HrContext context)
        {
            _context = context;
        }

        public void sendMessage(string name, string message)
        {
            //save in db 
            var chatMessage = new ChatMessage
            {
                Username = name,
                Message = message
            };
            _context.ChatMessages.Add(chatMessage);
            _context.SaveChanges();
            //broadcast to all clients
            Clients.All.SendAsync("NewMessage", name, message);
        }
    }
}


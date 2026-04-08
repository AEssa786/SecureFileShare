using Microsoft.AspNetCore.SignalR;

namespace SecureFileShare.Data.SignalR
{
    public class chatHub : Hub
    {

        private readonly SecureFileShareContext _context;

        public chatHub(SecureFileShareContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string senderId, string recipientId, string message)
        {
            var newMessage = new Models.Message
            {
                SenderId = senderId,
                RecipientId = recipientId,
                Content = message,
                Timestamp = DateTime.Now
            };
            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();
            await Clients.User(recipientId).SendAsync("ReceiveMessage", senderId, message, newMessage.Timestamp.ToString("o"));
            await Clients.User(senderId).SendAsync("ReceiveMessage", senderId, message, newMessage.Timestamp.ToString("o"));
        }

    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SecureFileShare.Models;

namespace SecureFileShare.Data.SignalR
{
    public class chatHub : Hub
    {

        private readonly SecureFileShareContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public chatHub(SecureFileShareContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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

            var sender = await _userManager.FindByIdAsync(senderId);
            var recipient = await _userManager.FindByIdAsync(recipientId);

            var senderName = $"{sender.FirstName} {sender.LastName}";
            var recipientName = $"{recipient.FirstName} {recipient.LastName}";

            
            await Clients.User(recipientId).SendAsync("ReceiveMessage", senderId, senderName, message, newMessage.Timestamp.ToString("o"));
            await Clients.User(senderId).SendAsync("ReceiveMessage", senderId, recipientName, message, newMessage.Timestamp.ToString("o"));
        }

    }
}

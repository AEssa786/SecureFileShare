using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SecureFileShare.Models;
using SecureFileShare.Services;

namespace SecureFileShare.Data.SignalR
{
    public class chatHub : Hub
    {
        private readonly IChatService _chatService;

        public chatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task SendMessage(string recipientId, string message)
        {
            var senderId = Context.UserIdentifier;

            var messageDto = await _chatService.CreateAndSaveMessage(senderId, recipientId, message);

            await Clients.User(recipientId).SendAsync("ReceiveMessage", messageDto);
            await Clients.User(senderId).SendAsync("ReceiveMessage", messageDto);
        }

        public async Task MarkAsRead(int messageId)
        {
            if (messageId > 0) { 
                string senderId = await _chatService.MarkMessageAsRead(messageId);
                await Clients.User(senderId).SendAsync("MessageRead", messageId);
            }
        }

        public async Task MarkAllRead(string otherUserId)
        {
            var userId = Context.UserIdentifier;
            await _chatService.MarkAllRead(userId, otherUserId);
            await Clients.User(otherUserId).SendAsync("AllMessagesRead", userId);
        }

    }
}

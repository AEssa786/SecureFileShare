
using SecureFileShare.Data.DataTransferObjects;
using SecureFileShare.Data.RepositoryPattern;
using SecureFileShare.Models;

namespace SecureFileShare.Services
{
    public class ChatService : IChatService
    {

        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;

        public ChatService(IChatRepository chatRepository, IUserRepository userRepository)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }

        public async Task<ChatDTO> CreateAndSaveMessage(string senderId, string recipientId, string content)
        {
            var msg = new Message
            {
                SenderId = senderId,
                RecipientId = recipientId,
                Content = content,
                Timestamp = DateTime.Now
            };

            await _chatRepository.AddMessageAsync(msg);

            var sender = await _userRepository.getUserByIdAsync(senderId);

            return new ChatDTO
            {
                SenderId = senderId,
                Content = content,
                Timestamp = msg.Timestamp.ToString("o"),
                SenderName = $"{sender.FirstName} {sender.LastName}",
                messageId = msg.MessageId,
                isRead = msg.IsRead
            };

        }

        public async Task MarkAllRead(string userId, string otherId)
        {
            var messages = await _chatRepository.getUnreadMessagesAsync(userId, otherId);

            if (messages != null)
            {
                foreach (var message in messages)
                {
                    message.IsRead = true;
                }
                await _chatRepository.saveChanges();
            }

        }

        public async Task<string> MarkMessageAsRead(int messageId)
        {
            var message = await _chatRepository.getById(messageId);
            if (message != null)
            {
                message.IsRead = true;
                await _chatRepository.saveChanges();
                return message.SenderId;
            }
            return null;
        }

        

    }
}

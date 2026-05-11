
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
                SenderName = $"{sender.FirstName} {sender.LastName}"
            };

        }
    }
}

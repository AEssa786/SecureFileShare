using SecureFileShare.Data.DataTransferObjects;
using SecureFileShare.Models;

namespace SecureFileShare.Data.RepositoryPattern
{
    public interface IChatRepository
    {

        Task<IEnumerable<Message>> getAllAsync(string id);
        Task<IEnumerable<ChatDTO>> loadHistoryAsync(string senderId, string recipientId);
        Task<Message> getById(int id);
        Task AddMessageAsync(Message message);
        Task AddAttachmentAsync(MessageAttachment attachment);
        Task UpdateMessage(Message message);
        Task<IEnumerable<Message>> getUnreadMessagesAsync(string userId, string otherId);
        Task saveChanges();

    }
}

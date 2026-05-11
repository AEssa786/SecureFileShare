using SecureFileShare.Data.DataTransferObjects;
using SecureFileShare.Models;

namespace SecureFileShare.Data.RepositoryPattern
{
    public interface IChatRepository
    {

        Task<IEnumerable<Message>> getAllAsync(string id);
        Task<IEnumerable<ChatDTO>> loadHistoryAsync(string senderId, string recipientId);
        Task AddMessageAsync(Message message);
        Task AddAttachmentAsync(MessageAttachment attachment);

    }
}

using SecureFileShare.Data.DataTransferObjects;

namespace SecureFileShare.Services
{
    public interface IChatService
    {

        Task<ChatDTO> CreateAndSaveMessage(string senderId, string recipientId, string content);
        Task<string> MarkMessageAsRead(int messageId);
        Task MarkAllRead(string userId, string otherId);

    }
}

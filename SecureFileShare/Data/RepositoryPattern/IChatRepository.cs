using SecureFileShare.Models;

namespace SecureFileShare.Data.RepositoryPattern
{
    public interface IChatRepository
    {

        Task<IEnumerable<Message>> getAllAsync(string id);
        Task<IEnumerable<Message>> loadHistoryAsync(string senderId, string recipientId);

    }
}

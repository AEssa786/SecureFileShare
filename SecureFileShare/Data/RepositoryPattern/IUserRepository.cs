using SecureFileShare.Models;

namespace SecureFileShare.Data.RepositoryPattern
{
    public interface IUserRepository
    {

        Task<IEnumerable<ApplicationUser>> searchUsersAsync(string query);
        Task <IEnumerable<ApplicationUser>> getAllUsers();
        Task <IEnumerable<ApplicationUser>> getChatUsers(IEnumerable<Message> messages);
        Task <ApplicationUser> getUserByIdAsync(string id);
    }
}

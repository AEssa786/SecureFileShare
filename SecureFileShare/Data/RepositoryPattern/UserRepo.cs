using Microsoft.EntityFrameworkCore;
using SecureFileShare.Models;

namespace SecureFileShare.Data.RepositoryPattern
{
    public class UserRepo : IUserRepository
    {

        private readonly SecureFileShareContext _context;
        public UserRepo(SecureFileShareContext context)
        {
            _context = context;
        }

        public Task<IEnumerable<ApplicationUser>> getAllUsers()
        {
            throw new NotImplementedException();
        }

        // This method takes a list of messages, extracts the unique sender and recipient IDs, and
        // then queries the database to retrieve the corresponding ApplicationUser records for those IDs.
        public async Task<IEnumerable<ApplicationUser>> getChatUsers(IEnumerable<Message> messages)
        {
            // Get the unique IDs
            var userIds = messages.Select(m => m.SenderId)
                         .Union(messages.Select(m => m.RecipientId))
                         .Distinct();

            // Fetch the users from the database who have these IDs
            return await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();
        }

        public async Task<ApplicationUser> getUserByIdAsync(string id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        // This method performs a search for users based on a query string, checking if the query is contained in the
        // username, first name, or last name of the users in the database. It returns a list of matching ApplicationUser records.
        public async Task<IEnumerable<ApplicationUser>> searchUsersAsync(string query)
        {
            return await _context.Users.Where(u => u.UserName.Contains(query)
            || u.FirstName.Contains(query) || u.LastName.Contains(query)).ToListAsync();
        }
    }
}

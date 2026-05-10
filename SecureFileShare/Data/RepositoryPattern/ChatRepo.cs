using Microsoft.EntityFrameworkCore;
using SecureFileShare.Models;

namespace SecureFileShare.Data.RepositoryPattern
{
    public class ChatRepo : IChatRepository
    {

        private readonly SecureFileShareContext _context;
        public ChatRepo(SecureFileShareContext context)
        {
            _context = context;
        }

        // This method retrieves all messages for a given user ID, where the user is either the sender or the recipient of the message.
        public async Task<IEnumerable<Message>> getAllAsync(string id)
        {
            return await _context.Messages
                    .Where(m => m.SenderId == id || m.RecipientId == id)
                    .ToListAsync();
        }

        // This method retrieves the chat history between two users, identified by their user IDs.
        // It fetches messages where one user is the sender and the other is the recipient, and orders the messages
        // by their timestamp to maintain the conversation flow.
        public async Task<IEnumerable<Message>> loadHistoryAsync(string currentUser, string recipientId)
        {
            return await _context.Messages
                .Where(m => (m.SenderId == currentUser && m.RecipientId == recipientId) ||
                            (m.SenderId == recipientId && m.RecipientId == currentUser))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SecureFileShare.Data.DataTransferObjects;
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

        public async Task AddAttachmentAsync(MessageAttachment attachment)
        {
            await _context.MessageAttachments.AddAsync(attachment);
            await _context.SaveChangesAsync();
        }

        public async Task AddMessageAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        // This method retrieves all messages for a given user ID, where the user is either the sender or the recipient of the message.
        public async Task<IEnumerable<Message>> getAllAsync(string id)
        {
            return await _context.Messages
                    .Where(m => m.SenderId == id || m.RecipientId == id)
                    .ToListAsync();
        }

        public async Task<Message> getById(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<IEnumerable<Message>> getUnreadMessagesAsync(string userId, string otherId)
        {
            return await _context.Messages
                .Where(m => m.RecipientId == userId && m.SenderId == otherId && !m.IsRead)
                .ToListAsync();
        }

        // This method retrieves the chat history between two users, identified by their user IDs.
        // It fetches messages where one user is the sender and the other is the recipient, and orders the messages
        // by their timestamp to maintain the conversation flow.
        public async Task<IEnumerable<ChatDTO>> loadHistoryAsync(string currentUser, string recipientId)
        {
            return await _context.Messages
                .Where(m => (m.SenderId == currentUser && m.RecipientId == recipientId) ||
                            (m.SenderId == recipientId && m.RecipientId == currentUser))
                .OrderBy(m => m.Timestamp)
                .Select(m => new ChatDTO
                {
                    SenderId = m.SenderId,
                    Content = m.Content,
                    Timestamp = m.Timestamp.ToString("o"), // ISO format for easy JS parsing
                    isRead = m.IsRead,                                       // We look into the Attachments table to find a matching FileId for this message
                    FileURL = _context.MessageAttachments
                    .Where(a => a.MessageId == m.MessageId)
                    .Select(a => $"/Chat/DownloadFile?fileId={a.FileId}")
                    .FirstOrDefault()
                })
                .ToListAsync();
        }

        public async Task UpdateMessage(Message message)
        {
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
        }
        public async Task saveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}

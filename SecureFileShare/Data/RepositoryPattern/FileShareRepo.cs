

using Microsoft.EntityFrameworkCore;

namespace SecureFileShare.Data.RepositoryPattern
{
    public class FileShareRepo : IFileShareRepository
    {
        private readonly SecureFileShareContext _context;

        public FileShareRepo(SecureFileShareContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Models.FileShare>> getAllAsync(string userId)
        {
            return await _context.FileShares
                .Include(fs => fs.SharedFile)
                .Include(fs => fs.SharedWith)
                .Include(fs => fs.SharedFrom)
                .Where(fs => fs.SharedWithId == userId || fs.SharedFromId == userId )
                .ToListAsync();
        }

        public async Task shareFile(Models.File file, string sharedWithId, string sharedFromId)
        {
           await _context.FileShares.AddAsync(new Models.FileShare
            {
                SharedFileId = file.FileId,
                SharedWithId = sharedWithId,
                SharedFromId = sharedFromId,
            });
            await _context.SaveChangesAsync();
        }
    }
}

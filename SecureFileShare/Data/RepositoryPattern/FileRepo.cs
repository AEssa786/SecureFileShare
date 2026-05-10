using Microsoft.EntityFrameworkCore;

namespace SecureFileShare.Data.RepositoryPattern
{
    public class FileRepo : IFileRepository
    {

        private readonly SecureFileShareContext _context;


        public FileRepo(SecureFileShareContext context)
        {
            _context = context;
        }

        //Adding a file to the database, after the file is uploaded to the server so that there is a file path to save in the database
        public async Task addAsync(Models.File entity)
        {
            await _context.Files.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        //Deleting a file from the database, after the file is deleted from the server so that there is no invalid file path in the database
        public async Task deleteAsync(int id)
        {
            var file = await _context.Files.FindAsync(id);
            if (file != null)
            {
                _context.Files.Remove(file);
                await _context.SaveChangesAsync();
            }
        }

        //Getting all files for a specific user, including the owner information for each file
        public async Task<IEnumerable<Models.File>> getAllAsync(string id)
        {
            return await _context.Files
                .Include(u => u.Owner)
                .Where(u => u.OwnerId == id)
                .ToListAsync();
        }

        //Getting a specific file by its ID, including the owner information for the file
        public async Task<Models.File> getByIdAsync(int id)
        {
            return await _context.Files
                .Include(u => u.Owner)
                .FirstOrDefaultAsync(f => f.FileId == id);
        }
    }
}

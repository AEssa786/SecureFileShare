using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecureFileShare.Models;

namespace SecureFileShare.Data.RepositoryPattern
{
    public class FileRepo : IRepository<Models.File>
    {

        private readonly SecureFileShareContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FileRepo(SecureFileShareContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task addAsync(Models.File entity)
        {
            await _context.Files.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task deleteAsync(int id)
        {
            var file = await _context.Files.FindAsync(id);
            if (file != null)
            {
                _context.Files.Remove(file);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Models.File>> getAllAsync(string id)
        {
            return await _context.Files
                .Include(u => u.Owner)
                .Where(u => u.OwnerId == id)
                .ToListAsync();
        }

        public  Task<Models.File> getByIdAsync(int id)
        {
            return _context.Files
                .Include(u => u.Owner)
                .FirstOrDefaultAsync(f => f.FileId == id);
        }
    }
}

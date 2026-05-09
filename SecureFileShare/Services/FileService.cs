
namespace SecureFileShare.Services
{
    public class FileService : IFileService
    {
        public FileStream DownloadFile(Models.File file)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.FilePath);
            var fileName = file.FileName;

            var fileStream = new FileStream(
                filePath, 
                FileMode.Open, 
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 9000,
                useAsync: true);

            return fileStream;
        }

        public async Task<string> SaveFileBytesAsync(byte[] fileBytes, string fileName)
        {
            throw new NotImplementedException();
        }

        public bool fileCheck(IFormFile file, string fileName)
        {
            if (file == null || file.Length == 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }

            if (file.Length > 10 * 1024 * 1024) // Limit file size to 10 MB
            {
                return false;
            }

            return true;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string fileName)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine("uploads", uniqueFileName).Replace("\\", "/");
        }

        public async Task DeleteFile(string path)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}

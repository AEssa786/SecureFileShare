using SecureFileShare.Models;

namespace SecureFileShare.Services
{
    public interface IFileService
    {

        Task<string> UploadFileAsync(IFormFile file, string fileName);
        FileStream DownloadFile(Models.File file);
        bool fileCheck(IFormFile file, string fileName);
        Task DeleteFile(string path);

    }
}

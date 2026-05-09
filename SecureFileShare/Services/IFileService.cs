using SecureFileShare.Models;

namespace SecureFileShare.Services
{
    public interface IFileService
    {

        Task<string> UploadFileAsync(IFormFile file, string fileName);
        FileStream DownloadFile(Models.File file);
        Task<string> SaveFileBytesAsync(byte[] fileBytes, string fileName);
        bool fileCheck(IFormFile file, string fileName);
        Task DeleteFile(string path);

    }
}

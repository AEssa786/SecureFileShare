
namespace SecureFileShare.Services
{
    public class FileService : IFileService
    {
        //This method takes a file record from the database, constructs the file path on the server, and
        //returns a FileStream for the file for downloading. The FileStream is opened with asynchronous
        //support and a buffer size of 9000 bytes for efficient file streaming.
        public FileStream DownloadFile(Models.File file)
        {
            //Construct the file path on the server using the current directory, the "Data" folder, and the file path stored in the database
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", file.FilePath);
            var fileName = file.FileName;

            //Open a FileStream for the file with asynchronous support and a buffer size of 9000 bytes
            var fileStream = new FileStream(
                filePath, 
                FileMode.Open, 
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 9000,
                useAsync: true);

            // Return the FileStream for downloading
            return fileStream;
        }

        //Basic Validation for the uploaded files
        public bool fileCheck(IFormFile file, string fileName)
        {
            //Checks if there is an actual file uploaded
            if (file == null || file.Length == 0)
            {
                return false;
            }

            //Check if there is a file name provided and that it is not empty or null
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }

            //Check if the file size exceeds the limit of 10 MB (10 * 1024 * 1024 bytes)
            if (file.Length > 10 * 1024 * 1024) // Limit file size to 10 MB
            {
                return false;
            }

            return true;
        }

        //This method handles the file upload process. It first checks if the "Uploads" folder exists in the
        //"Data" directory and creates it if it doesn't. Then, it generates a unique file name by combining
        //a new GUID with the original file name and its extension. The file is saved to the server using a
        //FileStream, and the method returns the relative path to the uploaded file for storing in the database.
        public async Task<string> UploadFileAsync(IFormFile file, string fileName)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Uploads");
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

        //Delete the file off the server
        public async Task DeleteFile(string path)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", path);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}

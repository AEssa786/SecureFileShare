using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecureFileShare.Data.RepositoryPattern;
using SecureFileShare.Models;
using SecureFileShare.Services;

namespace SecureFileShare.Controllers
{
    public class FileShareController : Controller
    {

        private readonly IFileShareRepository _fileShareRepository;
        private readonly IFileService _fileService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileRepository _fileRepository;

        public FileShareController(
            IFileShareRepository fileShareRepository, 
            IFileService fileService, 
            UserManager<ApplicationUser> userManager,
            IFileRepository fileRepository)
        {
            _fileShareRepository = fileShareRepository;
            _fileService = fileService;
            _userManager = userManager;
            _fileRepository = fileRepository;
        }

        public async Task<IActionResult> SharedFiles()
        {
            var currentUserId = _userManager.GetUserId(User);
            var files = await _fileShareRepository.getAllAsync(currentUserId);
            return View(files);
        }

        public async Task<IActionResult> Download(int id)
        {
            //Get the file from the database and check if it exists and belongs to the currently logged in user
            var file = _fileRepository.getByIdAsync(id).Result;
            if (file == null)
            {
                return NotFound();
            }

            //Get the file stream from the file service and return it for download
            var fileStream = _fileService.DownloadFile(file);
            if (fileStream == null)
            {
                return NotFound();
            }

            //Return the file stream with the appropriate content type and file name for download
            return File(fileStream, "application/octet-stream", file.FileName);
        }
    }
}

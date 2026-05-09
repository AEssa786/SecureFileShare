using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecureFileShare.Data;
using SecureFileShare.Data.RepositoryPattern;
using SecureFileShare.Models;
using SecureFileShare.Services;
using System.Threading.Tasks;

namespace SecureFileShare.Controllers
{
    public class FileController : Controller
    {

        private readonly IRepository<Models.File> _fileRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileService _fileService;

        public FileController(IRepository<Models.File> fileRepository, UserManager<ApplicationUser> userManager, IFileService fileService)
        {
            _userManager = userManager;
            _fileRepository = fileRepository;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> AllFiles()
        {
            var userId = _userManager.GetUserId(User);
            var files = await _fileRepository.getAllAsync(userId);
            return View(files);
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, string fileName)
        {
            if(!_fileService.fileCheck(file, fileName))
            {
                ModelState.AddModelError(string.Empty, "Invalid file or file name. Please ensure you have selected a file and provided a valid name. The file size must not exceed 10 MB.");
                return View("Upload");
            }

            else
            {
                var filePath = await _fileService.UploadFileAsync(file, fileName);

                await _fileRepository.addAsync(new Models.File
                {
                    FileName = fileName + Path.GetExtension(file.FileName),
                    FilePath = filePath,
                    OwnerId = _userManager.GetUserId(User),
                    Size = file.Length
                });
                return RedirectToAction("AllFiles");
            }
        }

        public async Task<IActionResult> Download(int id)
        {
            var file = _fileRepository.getByIdAsync(id).Result;
            if (file == null || file.OwnerId != _userManager.GetUserId(User))
            {
                return NotFound();
            }
            
            var fileStream = _fileService.DownloadFile(file);
            if (fileStream == null) { 
                return NotFound();
            }

            return File(fileStream, "application/octet-stream", file.FileName);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var file = _fileRepository.getByIdAsync(id).Result;
            if (file == null || file.OwnerId != _userManager.GetUserId(User))
            {
                return NotFound();
            }
            _fileService.DeleteFile(file.FilePath).Wait();
            _fileRepository.deleteAsync(id).Wait();
            return RedirectToAction("AllFiles");
        }

    }
}

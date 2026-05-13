using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecureFileShare.Data.RepositoryPattern;
using SecureFileShare.Models;
using SecureFileShare.Services;

namespace SecureFileShare.Controllers
{
    public class FileController : Controller
    {

        private readonly IFileRepository _fileRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileService _fileService;
        private readonly IFileShareRepository _fileShareRepository;
        private readonly IUserRepository _userRepository;

        public FileController(
            IFileRepository fileRepository, 
            UserManager<ApplicationUser> userManager, 
            IFileService fileService,
            IFileShareRepository fileShareRepository,
            IUserRepository userRepository)
        {
            _userManager = userManager;
            _fileRepository = fileRepository;
            _fileService = fileService;
            _fileShareRepository = fileShareRepository;
            _userRepository = userRepository;
        }

        //Fetch all files belonging to the currently logged in user and display them in the AllFiles view
        [HttpGet]
        public async Task<IActionResult> AllFiles()
        {
            var userId = _userManager.GetUserId(User);
            var files = await _fileRepository.getAllAsync(userId);
            return View(files);
        }

        //Display the file upload form to the user
        public IActionResult Upload()
        {
            return View();
        }

        //Handle the file upload form submission, validate the file and file name, save the file to the server,
        //and add a record to the database with the file information
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

        //Handle the file download request, validate that the file exists and belongs to the currently
        //logged in user, and return the file stream for download
        public async Task<IActionResult> Download(int id)
        {
            //Get the file from the database and check if it exists and belongs to the currently logged in user
            var file = _fileRepository.getByIdAsync(id).Result;
            if (file == null || file.OwnerId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            //Get the file stream from the file service and return it for download
            var fileStream = _fileService.DownloadFile(file);
            if (fileStream == null) { 
                return NotFound();
            }

            //Return the file stream with the appropriate content type and file name for download
            return File(fileStream, "application/octet-stream", file.FileName);
        }

        //Handle the file deletion request, validate that the file exists and belongs to the currently
        public async Task<IActionResult> Delete(int id)
        {
            //Get the file from the database and check if it exists and belongs to the currently logged in user
            var file = _fileRepository.getByIdAsync(id).Result;
            if (file == null || file.OwnerId != _userManager.GetUserId(User))
            {
                return NotFound();
            }
            //Delete the file from the server and remove the record from the database
            _fileService.DeleteFile(file.FilePath).Wait();
            _fileRepository.deleteAsync(id).Wait();
            return RedirectToAction("AllFiles");
        }

        public async Task<IActionResult> Share(int fileId, string recipientId)
        {
            var file = await _fileRepository.getByIdAsync(fileId);
            var userId = _userManager.GetUserId(User);

            if (file == null || file.OwnerId != userId)
            {
                return Json(new { success = false, message = "File not found or access denied." });
            }

            try
            {
                await _fileShareRepository.shareFile(file, recipientId, userId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult searchUsers(string query)
        {
            // Call the repository method to search for users based on the query string, which checks against username, first name, and last name.
            var users = _userRepository.searchUsersAsync(query).Result.Select(u => new
            {
                u.Id,
                u.UserName,
                u.FirstName,
                u.LastName
            });

            // Return the list of matching users as a JSON response to the client-side AJAX call, which will then display the results in the UI.
            return Json(users);
        }

    }
}

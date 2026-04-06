using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecureFileShare.Data;
using SecureFileShare.Models;

namespace SecureFileShare.Controllers
{
    public class FileController : Controller
    {

        private readonly SecureFileShareContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FileController(SecureFileShareContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult AllFiles()
        {
            var userId = _userManager.GetUserId(User);
            var files = _context.Files.Where(f => f.OwnerId == userId).ToList();
            return View(files);
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, string fileName)
        {
            if (file != null && file.Length > 0)
            {
                var name = fileName + Path.GetExtension(file.FileName);
                var filePath = Path.Combine("Data/Uploads/", name);

                if (!Directory.Exists("Data/Uploads/"))
                {
                    Directory.CreateDirectory("Data/Uploads/");
                }

                if (file.Length > 11 * 1024 * 1024) // Limit file size to 10 MB
                {
                    ModelState.AddModelError(string.Empty, "File size exceeds the 10 MB limit.");
                    return View("Upload");
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fileInfo = new FileInfo(filePath);

                Models.File newFile = new Models.File
                {
                    FileName = name,
                    FilePath = filePath,
                    OwnerId = _userManager.GetUserId(User),
                    Size = fileInfo.Length, // Size in Bytes
                };

                _context.Files.Add(newFile);
                await _context.SaveChangesAsync();

                return RedirectToAction("AllFiles");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Please select a file to upload.");
                return View("Upload");
            }
        }

        public async Task<IActionResult> Download(int id)
        {
            var file = await _context.Files.FindAsync(id);
            if (file == null || file.OwnerId != _userManager.GetUserId(User))
            {
                return NotFound();
            }
            var memory = new MemoryStream();
            using (var stream = new FileStream(file.FilePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/octet-stream", file.FileName);
        }

        
        public async Task<IActionResult> Delete(int id)
        {
            var file = await _context.Files.FindAsync(id);
            if (file == null || file.OwnerId != _userManager.GetUserId(User))
            {
                return NotFound();
            }
            System.IO.File.Delete(file.FilePath);
            _context.Files.Remove(file);
            await _context.SaveChangesAsync();
            return RedirectToAction("AllFiles");
        }

    }
}

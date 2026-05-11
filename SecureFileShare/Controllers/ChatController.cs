using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureFileShare.Data;
using SecureFileShare.Data.DataTransferObjects;
using SecureFileShare.Data.RepositoryPattern;
using SecureFileShare.Models;
using SecureFileShare.Services;
using SecureFileShare.Data.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace SecureFileShare.Controllers
{
    public class ChatController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFileService _fileService;
        private readonly IFileRepository _fileRepo;
        private readonly IHubContext<chatHub> _hubContext;

        public ChatController(
            UserManager<ApplicationUser> userManager, 
            IChatRepository repository, 
            IUserRepository userRepository, 
            IFileService fileService,
            IFileRepository fileRepository,
            IHubContext<chatHub> hubContext)
        {
            _userManager = userManager;
            _chatRepository = repository;
            _userRepository = userRepository;
            _fileService = fileService;
            _fileRepo = fileRepository;
            _hubContext = hubContext;
        }

        //On load, get all messages for the current user, extract unique user IDs, and fetch those users to display in the chat list. 
        public async Task<IActionResult> Home()
        {
            // Get the current user's ID
            var currentUser = _userManager.GetUserId(User);

            // Get all messages involving the current user (either as sender or recipient), extracting the other user's ID from each message.
            var chatUserIds = await _chatRepository.getAllAsync(currentUser);

            // Use the extracted user IDs to fetch the actual user details from the database, ensuring we get real user data instead of just IDs.
            var chatUsers = await _userRepository.getChatUsers(chatUserIds);

            // Pass the list of chat users to the view to display in the chat sidebar.
            return View(chatUsers.ToList());
        }

        // This method will be called via AJAX when the user types in the search box to find other users to chat with.
        // It returns a JSON list of users matching the search query.
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

        // This method will be called via AJAX to retrieve the chat history between the current user and a selected recipient.
        // It returns a JSON list of messages.
        [HttpGet]
        public async Task<IActionResult> GetMessages(string recipientId)
        {
            var currentUser = _userManager.GetUserId(User);
            var messages = await _chatRepository.loadHistoryAsync(currentUser, recipientId);
            return Json(messages);
        }

        [HttpPost]
        public async Task<IActionResult> SendFile(string recipientId, IFormFile file)
        {
            var senderId = _userManager.GetUserId(User);

            if (_fileService.fileCheck(file, file.FileName)){
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var filePath = await _fileService.UploadFileAsync(file, fileName);
                var trueFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", filePath);
                var message = new Message
                {
                    SenderId = senderId,
                    RecipientId = recipientId,
                    Content = $"[📎File] {file.FileName}",
                    Timestamp = DateTime.Now
                };
                await _chatRepository.AddMessageAsync(message);

                var fileRecord = new Models.File
                {
                    FileName = file.FileName,
                    FilePath = filePath,
                    Size = file.Length,
                    OwnerId = senderId,
                };
                await _fileRepo.addAsync(fileRecord);

                var attachment = new MessageAttachment
                {
                    MessageId = message.MessageId,
                    FileId = fileRecord.FileId
                };
                await _chatRepository.AddAttachmentAsync(attachment);

                var sender = await _userRepository.getUserByIdAsync(senderId);

                var chatDto = new ChatDTO { 
                    SenderId = senderId,
                    SenderName = $"{sender.FirstName} {sender.LastName}",
                    Content = message.Content,
                    Timestamp = message.Timestamp.ToString("o"),
                    FileURL = $"/Chat/DownloadFile?fileId={fileRecord.FileId}"
                };

                await _hubContext.Clients.User(recipientId).SendAsync("ReceiveMessage", chatDto);
                await _hubContext.Clients.User(senderId).SendAsync("ReceiveMessage", chatDto);

                return Ok(chatDto);

            }
            return BadRequest("Invalid file type or size.");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(int fileId)
        {
            var fileRecord = await _fileRepo.getByIdAsync(fileId);
            if (fileRecord == null)
            {
                return NotFound();
            }
            var fileStream = _fileService.DownloadFile(fileRecord);
            return File(fileStream, "application/octet-stream", fileRecord.FileName);
        }

    }
}

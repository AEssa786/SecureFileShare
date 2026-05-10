using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureFileShare.Data;
using SecureFileShare.Data.RepositoryPattern;
using SecureFileShare.Models;

namespace SecureFileShare.Controllers
{
    public class ChatController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;

        public ChatController(UserManager<ApplicationUser> userManager, IChatRepository repository, IUserRepository userRepository)
        {
            _userManager = userManager;
            _chatRepository = repository;
            _userRepository = userRepository;
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
            var messages = await _chatRepository.getAllAsync(currentUser);
            return Json(messages);
        }

    }
}

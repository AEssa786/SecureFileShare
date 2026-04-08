using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureFileShare.Data;
using SecureFileShare.Models;

namespace SecureFileShare.Controllers
{
    public class ChatController : Controller
    {

        private readonly SecureFileShareContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ChatController(SecureFileShareContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Home()
        {
            var currentUser = _userManager.GetUserId(User);
            
            var sentMessages = _context.Messages.Where(m => m.SenderId == currentUser)
                .Select(m => m.RecipientId);
            var receivedMessages = _context.Messages.Where(m => m.RecipientId == currentUser)
                .Select(m => m.SenderId);

            var chatUserIds = sentMessages.Union(receivedMessages).Distinct().ToList();

            var chatUsers = await _context.Users.Where(u => chatUserIds.Contains(u.Id)).ToListAsync();

            return View(chatUsers);
        }

        [HttpGet]
        public IActionResult searchUsers(string query)
        {
            var users = _context.Users.Where(u => u.UserName.Contains(query) 
            || u.FirstName.Contains(query) || u.LastName.Contains(query)).ToList();

            return Json(users);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages(string recipientId)
        {
            var currentUser = _userManager.GetUserId(User);
            var messages = await _context.Messages
                .Where(m => (m.SenderId == currentUser && m.RecipientId == recipientId) ||
                            (m.SenderId == recipientId && m.RecipientId == currentUser))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
            return Json(messages);
        }

    }
}

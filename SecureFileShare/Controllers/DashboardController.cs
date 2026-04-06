using Microsoft.AspNetCore.Mvc;

namespace SecureFileShare.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

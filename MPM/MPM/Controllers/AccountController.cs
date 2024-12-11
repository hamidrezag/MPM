using Microsoft.AspNetCore.Mvc;

namespace MPM.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

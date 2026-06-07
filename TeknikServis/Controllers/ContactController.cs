using Microsoft.AspNetCore.Mvc;

namespace TeknikServis.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
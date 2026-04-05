using Microsoft.AspNetCore.Mvc;

namespace CarServiceTracker.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult NotFound404()
        {
            return View();
        }

        public IActionResult Error500()
        {
            return View();
        }
    }
}
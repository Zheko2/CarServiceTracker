using CarServiceTracker.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarServiceTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.CarsCount = await _context.Cars.CountAsync();
            ViewBag.ServiceRecordsCount = await _context.ServiceRecords.CountAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}

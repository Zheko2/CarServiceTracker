using CarServiceTracker.Data;
using CarServiceTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarServiceTracker.Controllers
{
    [Authorize]
    public class ExpensesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ExpensesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole("Administrator");

            var expenses = await _context.Expenses
                .Include(e => e.Car)
                .Where(e => isAdmin || e.Car!.OwnerId == userId)
                .OrderByDescending(e => e.Date)
                .ToListAsync();

            return View(expenses);
        }

        public async Task<IActionResult> Create()
        {
            await LoadCarsAsync();
            LoadCategories();
            return View(new Expense { Date = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expense expense)
        {
            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole("Administrator");

            var allowedCar = await _context.Cars
                .AnyAsync(c => c.Id == expense.CarId && (isAdmin || c.OwnerId == userId));

            if (!allowedCar)
            {
                ModelState.AddModelError(nameof(expense.CarId), "Invalid car selection.");
            }

            if (!ModelState.IsValid)
            {
                await LoadCarsAsync(expense.CarId);
                LoadCategories(expense.Category);
                return View(expense);
            }

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCarsAsync(int? selectedCarId = null)
        {
            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole("Administrator");

            var cars = await _context.Cars
                .Where(c => isAdmin || c.OwnerId == userId)
                .OrderBy(c => c.Brand)
                .ThenBy(c => c.Model)
                .Select(c => new
                {
                    c.Id,
                    Text = c.Brand + " " + c.Model + " (" + c.Year + ")"
                })
                .ToListAsync();

            ViewData["CarId"] = new SelectList(cars, "Id", "Text", selectedCarId);
        }

        private void LoadCategories(string? selectedCategory = null)
        {
            var categories = new List<string>
            {
                "Fuel",
                "Insurance",
                "Tax",
                "Car Wash",
                "Parking",
                "Vignette",
                "Accessories",
                "Other"
            };

            ViewData["Category"] = new SelectList(categories, selectedCategory);
        }
    }
}
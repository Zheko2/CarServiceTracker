using CarServiceTracker.Data;
using CarServiceTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarServiceTracker.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpensesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var expenses = await _context.Expenses
                .Include(e => e.Car)
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
            var cars = await _context.Cars
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
using CarServiceTracker.Data;
using CarServiceTracker.Models;
using CarServiceTracker.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarServiceTracker.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly IExpenseService _expenseService;
        private readonly ApplicationDbContext _context;

        public ExpensesController(IExpenseService expenseService, ApplicationDbContext context)
        {
            _expenseService = expenseService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var expenses = await _expenseService.GetAllAsync();
            return View(expenses);
        }

        public async Task<IActionResult> Create()
        {
            await LoadCarsAsync();
            return View(new Expense { Date = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expense expense)
        {
            if (!ModelState.IsValid)
            {
                await LoadCarsAsync(expense.CarId);
                return View(expense);
            }

            await _expenseService.CreateAsync(expense);
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
    }
}
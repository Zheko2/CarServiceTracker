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
            var cars = await _context.Cars
                .OrderBy(c => c.Brand)
                .ThenBy(c => c.Model)
                .Select(c => new
                {
                    c.Id,
                    Text = c.Brand + " " + c.Model + " (" + c.Year + ")"
                })
                .ToListAsync();

            ViewData["CarId"] = new SelectList(cars, "Id", "Text");
            return View(new Expense { Date = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expense expense)
        {
            if (!ModelState.IsValid)
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

                ViewData["CarId"] = new SelectList(cars, "Id", "Text", expense.CarId);
                return View(expense);
            }

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
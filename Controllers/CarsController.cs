using CarServiceTracker.Data;
using CarServiceTracker.Models;
using CarServiceTracker.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarServiceTracker.Controllers
{
    [Authorize]
    public class CarsController : Controller
    {
        private readonly ICarService _carService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CarsController(
            ICarService carService,
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _carService = carService;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? searchTerm)
        {
            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole("Administrator");

            var cars = await _context.Cars
                .Include(c => c.ServiceRecords)
                .Include(c => c.Garage)
                .Where(c => isAdmin || c.OwnerId == userId)
                .AsQueryable()
                .Where(c => string.IsNullOrWhiteSpace(searchTerm) ||
                            c.Brand.Contains(searchTerm) ||
                            c.Model.Contains(searchTerm))
                .OrderBy(c => c.Brand)
                .ThenBy(c => c.Model)
                .ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            return View(cars);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole("Administrator");

            var car = await _context.Cars
                .Include(c => c.ServiceRecords)
                .Include(c => c.Garage)
                .FirstOrDefaultAsync(c => c.Id == id && (isAdmin || c.OwnerId == userId));

            if (car == null)
                return NotFound();

            return View(car);
        }

        public async Task<IActionResult> Create()
        {
            await LoadGaragesAsync();
            LoadStatuses();
            return View(new Car { Status = "Repairing" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Car car)
        {
            car.OwnerId = _userManager.GetUserId(User)!;

            ModelState.Remove(nameof(Car.OwnerId));

            if (!ModelState.IsValid)
            {
                await LoadGaragesAsync(car.GarageId);
                LoadStatuses(car.Status);
                return View(car);
            }

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole("Administrator");

            var car = await _context.Cars
                .FirstOrDefaultAsync(c => c.Id == id && (isAdmin || c.OwnerId == userId));

            if (car == null)
                return NotFound();

            await LoadGaragesAsync(car.GarageId);
            LoadStatuses(car.Status);
            return View(car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Car car)
        {
            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole("Administrator");

            if (id != car.Id)
                return BadRequest();

            var existingCar = await _context.Cars
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id && (isAdmin || c.OwnerId == userId));

            if (existingCar == null)
                return NotFound();

            car.OwnerId = existingCar.OwnerId;
            ModelState.Remove(nameof(Car.OwnerId));

            if (!ModelState.IsValid)
            {
                await LoadGaragesAsync(car.GarageId);
                LoadStatuses(car.Status);
                return View(car);
            }

            _context.Update(car);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var car = await _context.Cars
                .Include(c => c.Garage)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
                return NotFound();

            return View(car);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
                return NotFound();

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadGaragesAsync(int? selectedGarageId = null)
        {
            var garages = await _context.Garages
                .OrderBy(g => g.Name)
                .Select(g => new
                {
                    g.Id,
                    Text = g.Name + " - " + g.Location
                })
                .ToListAsync();

            ViewData["GarageId"] = new SelectList(garages, "Id", "Text", selectedGarageId);
        }

        private void LoadStatuses(string? selectedStatus = null)
        {
            var statuses = new List<string>
            {
                "Repairing",
                "Changing",
                "Repaired",
                "Changed"
            };

            ViewData["Status"] = new SelectList(statuses, selectedStatus);
        }
    }
}
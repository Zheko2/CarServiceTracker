using CarServiceTracker.Data;
using CarServiceTracker.Models;
using CarServiceTracker.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarServiceTracker.Controllers
{
    public class CarsController : Controller
    {
        private readonly ICarService _carService;
        private readonly ApplicationDbContext _context;

        public CarsController(ICarService carService, ApplicationDbContext context)
        {
            _carService = carService;
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchTerm)
        {
            var cars = await _carService.GetAllAsync(searchTerm);
            ViewBag.SearchTerm = searchTerm;
            return View(cars);
        }

        public async Task<IActionResult> Details(int id)
        {
            var car = await _context.Cars
                .Include(c => c.ServiceRecords)
                .Include(c => c.Garage)
                .FirstOrDefaultAsync(c => c.Id == id);

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
            if (!ModelState.IsValid)
            {
                await LoadGaragesAsync(car.GarageId);
                LoadStatuses(car.Status);
                return View(car);
            }

            await _carService.CreateAsync(car);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var car = await _context.Cars.FindAsync(id);

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
            if (id != car.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await LoadGaragesAsync(car.GarageId);
                LoadStatuses(car.Status);
                return View(car);
            }

            await _carService.UpdateAsync(car);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var car = await _carService.GetByIdAsync(id);

            if (car == null)
                return NotFound();

            return View(car);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _carService.DeleteAsync(id);
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
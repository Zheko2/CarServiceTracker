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
    public class ServiceRecordsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ServiceRecordsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? carId, int page = 1)
        {
            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole("Administrator");
            int pageSize = 5;

            var query = _context.ServiceRecords
                .Include(r => r.Car)
                .Include(r => r.ServiceType)
                .Where(r => isAdmin || r.Car!.OwnerId == userId)
                .AsQueryable();

            if (carId.HasValue)
            {
                query = query.Where(r => r.CarId == carId.Value);

                var car = await _context.Cars
                    .Where(c => c.Id == carId.Value && (isAdmin || c.OwnerId == userId))
                    .Select(c => new { c.Brand, c.Model, c.Year })
                    .FirstOrDefaultAsync();

                if (car == null)
                {
                    return NotFound();
                }

                ViewBag.FilterCar = $"{car.Brand} {car.Model} ({car.Year})";
            }

            int totalRecords = await query.CountAsync();

            var records = await query
                .OrderByDescending(r => r.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

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

            ViewBag.Cars = cars;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            ViewBag.CurrentCarId = carId;

            return View(records);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole("Administrator");

            var record = await _context.ServiceRecords
                .Include(r => r.Car)
                .Include(r => r.ServiceType)
                .FirstOrDefaultAsync(r => r.Id == id && (isAdmin || r.Car!.OwnerId == userId));

            if (record == null) return NotFound();

            return View(record);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();
            return View(new ServiceRecord { Date = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRecord record)
        {
            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole("Administrator");

            var allowedCar = await _context.Cars
                .AnyAsync(c => c.Id == record.CarId && (isAdmin || c.OwnerId == userId));

            if (!allowedCar)
            {
                ModelState.AddModelError(nameof(record.CarId), "Invalid car selection.");
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(record.CarId, record.ServiceTypeId);
                return View(record);
            }

            _context.ServiceRecords.Add(record);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole("Administrator");

            var record = await _context.ServiceRecords
                .FirstOrDefaultAsync(r => r.Id == id && (isAdmin || r.Car!.OwnerId == userId));

            if (record == null) return NotFound();

            await LoadDropdownsAsync(record.CarId, record.ServiceTypeId);
            return View(record);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceRecord record)
        {
            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole("Administrator");

            if (id != record.Id) return NotFound();

            var existingRecord = await _context.ServiceRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id && (isAdmin || r.Car!.OwnerId == userId));

            if (existingRecord == null)
            {
                return NotFound();
            }

            var allowedCar = await _context.Cars
                .AnyAsync(c => c.Id == record.CarId && (isAdmin || c.OwnerId == userId));

            if (!allowedCar)
            {
                ModelState.AddModelError(nameof(record.CarId), "Invalid car selection.");
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(record.CarId, record.ServiceTypeId);
                return View(record);
            }

            _context.Update(record);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var record = await _context.ServiceRecords
                .Include(r => r.Car)
                .Include(r => r.ServiceType)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (record == null) return NotFound();

            return View(record);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var record = await _context.ServiceRecords.FindAsync(id);

            if (record != null)
            {
                _context.ServiceRecords.Remove(record);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdownsAsync(int? selectedCarId = null, int? selectedServiceTypeId = null)
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

            var serviceTypes = await _context.ServiceTypes
                .OrderBy(st => st.Name)
                .ToListAsync();

            ViewData["CarId"] = new SelectList(cars, "Id", "Text", selectedCarId);
            ViewData["ServiceTypeId"] = new SelectList(serviceTypes, "Id", "Name", selectedServiceTypeId);
        }
    }
}
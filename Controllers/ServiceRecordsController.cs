using CarServiceTracker.Data;
using CarServiceTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarServiceTracker.Controllers
{
    public class ServiceRecordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServiceRecordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // INDEX (with optional filter by carId)
        public async Task<IActionResult> Index(int? carId)
        {
            var query = _context.ServiceRecords
                .Include(r => r.Car)
                .Include(r => r.ServiceType)
                .AsQueryable();

            if (carId.HasValue)
            {
                query = query.Where(r => r.CarId == carId.Value);

                var car = await _context.Cars
                    .Where(c => c.Id == carId.Value)
                    .Select(c => new { c.Brand, c.Model, c.Year })
                    .FirstOrDefaultAsync();

                ViewBag.FilterCar = car != null
                    ? $"{car.Brand} {car.Model} ({car.Year})"
                    : "Selected car";
            }

            var records = await query
                .OrderByDescending(r => r.Date)
                .ToListAsync();

            return View(records);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var record = await _context.ServiceRecords
                .Include(r => r.Car)
                .Include(r => r.ServiceType)
                .FirstOrDefaultAsync(r => r.Id == id);

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

            var record = await _context.ServiceRecords.FindAsync(id);
            if (record == null) return NotFound();

            await LoadDropdownsAsync(record.CarId, record.ServiceTypeId);
            return View(record);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceRecord record)
        {
            if (id != record.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(record.CarId, record.ServiceTypeId);
                return View(record);
            }

            _context.Update(record);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

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
            var cars = await _context.Cars
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

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

        // GET: ServiceRecords
        public async Task<IActionResult> Index()
        {
            var records = await _context.ServiceRecords
                .Include(r => r.Car)
                .Include(r => r.ServiceType)
                .OrderByDescending(r => r.Date)
                .ToListAsync();

            return View(records);
        }

        // GET: ServiceRecords/Details/5
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

        // GET: ServiceRecords/Create
        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();
            return View();
        }

        // POST: ServiceRecords/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRecord record)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(record.CarId, record.ServiceTypeId);
                return View(record);
            }

            _context.Add(record);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: ServiceRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var record = await _context.ServiceRecords.FindAsync(id);
            if (record == null) return NotFound();

            await LoadDropdownsAsync(record.CarId, record.ServiceTypeId);
            return View(record);
        }

        // POST: ServiceRecords/Edit/5
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

            try
            {
                _context.Update(record);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceRecordExists(record.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: ServiceRecords/Delete/5
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

        // POST: ServiceRecords/Delete/5
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

        private bool ServiceRecordExists(int id)
            => _context.ServiceRecords.Any(e => e.Id == id);

        private async Task LoadDropdownsAsync(int? selectedCarId = null, int? selectedServiceTypeId = null)
        {
            var cars = await _context.Cars
                .OrderBy(c => c.Brand)
                .ThenBy(c => c.Model)
                .Select(c => new
                {
                    c.Id,
                    Text = c.Brand + " " + c.Model
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

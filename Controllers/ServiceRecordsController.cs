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

        // INDEX
        public async Task<IActionResult> Index()
        {
            var records = await _context.ServiceRecords
                .Include(r => r.Car)
                .Include(r => r.ServiceType)
                .OrderByDescending(r => r.Date)
                .ToListAsync();

            return View(records);
        }

        // CREATE GET
        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();
            return View(new ServiceRecord { Date = DateTime.Today });
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRecord record)
        {
            if (record.CarId <= 0)
                ModelState.AddModelError(nameof(record.CarId), "Please select a car.");

            if (record.ServiceTypeId <= 0)
                ModelState.AddModelError(nameof(record.ServiceTypeId), "Please select a service type.");

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(record.CarId, record.ServiceTypeId);
                return View(record);
            }

            _context.ServiceRecords.Add(record);
            await _context.SaveChangesAsync();

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

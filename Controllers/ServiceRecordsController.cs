using CarServiceTracker.Data;
using CarServiceTracker.Models;
using CarServiceTracker.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarServiceTracker.Controllers
{
    public class ServiceRecordsController : Controller
    {
        private readonly IServiceRecordService _serviceRecordService;
        private readonly ApplicationDbContext _context;

        public ServiceRecordsController(IServiceRecordService serviceRecordService, ApplicationDbContext context)
        {
            _serviceRecordService = serviceRecordService;
            _context = context;
        }

        public async Task<IActionResult> Index(int? carId, int page = 1)
        {
            int pageSize = 5;

            var records = await _serviceRecordService.GetAllAsync(carId, page, pageSize);
            int totalRecords = await _serviceRecordService.GetCountAsync(carId);

            if (carId.HasValue)
            {
                ViewBag.FilterCar = await _serviceRecordService.GetFilterCarNameAsync(carId.Value) ?? "Selected car";
            }

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            ViewBag.CurrentCarId = carId;

            var cars = await _context.Cars
             .OrderBy(c => c.Brand)
             .ThenBy(c => c.Model)
             .Select(c => new
            {
              c.Id,
              Text = c.Brand + " " + c.Model + " (" + c.Year + ")"
            })
            .ToListAsync();

            ViewBag.Cars = cars;

            return View(records);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var record = await _serviceRecordService.GetByIdAsync(id.Value);

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

            await _serviceRecordService.CreateAsync(record);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var record = await _serviceRecordService.GetByIdAsync(id.Value);
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

            await _serviceRecordService.UpdateAsync(record);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var record = await _serviceRecordService.GetByIdAsync(id.Value);

            if (record == null) return NotFound();

            return View(record);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _serviceRecordService.DeleteAsync(id);
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
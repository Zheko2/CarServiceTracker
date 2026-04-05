using CarServiceTracker.Models;
using CarServiceTracker.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceTracker.Controllers
{
    public class GarageController : Controller
    {
        private readonly IGarageService _garageService;

        public GarageController(IGarageService garageService)
        {
            _garageService = garageService;
        }

        // INDEX
        public async Task<IActionResult> Index()
        {
            var garages = await _garageService.GetAllAsync();
            return View(garages);
        }

        // DETAILS
        public async Task<IActionResult> Details(int id)
        {
            var garage = await _garageService.GetByIdAsync(id);

            if (garage == null)
                return NotFound();

            return View(garage);
        }

        // CREATE GET
        public IActionResult Create()
        {
            return View();
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Garage garage)
        {
            if (!ModelState.IsValid)
                return View(garage);

            await _garageService.CreateAsync(garage);
            return RedirectToAction(nameof(Index));
        }

        // DELETE GET
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var garage = await _garageService.GetByIdAsync(id);

            if (garage == null)
                return NotFound();

            return View(garage);
        }

        // DELETE POST
        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _garageService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
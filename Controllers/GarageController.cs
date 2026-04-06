using CarServiceTracker.Data;
using CarServiceTracker.Models;
using CarServiceTracker.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarServiceTracker.Controllers
{
    [Authorize]
    public class GarageController : Controller
    {
        private readonly IGarageService _garageService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public GarageController(
            IGarageService garageService,
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _garageService = garageService;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var garages = await _context.Garages
                .Include(g => g.Cars)
                .OrderBy(g => g.Name)
                .ToListAsync();

            return View(garages);
        }

        public async Task<IActionResult> Details(int id)
        {
            var garage = await _context.Garages
                .Include(g => g.Cars)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (garage == null)
                return NotFound();

            return View(garage);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Garage garage)
        {
            if (!ModelState.IsValid)
                return View(garage);

            garage.OwnerId = _userManager.GetUserId(User)!;

            _context.Garages.Add(garage);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var garage = await _context.Garages
                .Include(g => g.Cars)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (garage == null)
                return NotFound();

            return View(garage);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var garage = await _context.Garages.FindAsync(id);

            if (garage != null)
            {
                _context.Garages.Remove(garage);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
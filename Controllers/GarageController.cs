using CarServiceTracker.Data;
using CarServiceTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarServiceTracker.Controllers
{
    public class GarageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GarageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // INDEX
        public async Task<IActionResult> Index()
        {
            var garages = await _context.Garages
                .Include(g => g.Cars)
                .ToListAsync();

            return View(garages);
        }

        // DETAILS
        public async Task<IActionResult> Details(int id)
        {
            var garage = await _context.Garages
                .Include(g => g.Cars)
                .FirstOrDefaultAsync(g => g.Id == id);

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

            _context.Garages.Add(garage);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // DELETE GET
        public async Task<IActionResult> Delete(int id)
        {
            var garage = await _context.Garages.FindAsync(id);

            if (garage == null)
                return NotFound();

            return View(garage);
        }

        // DELETE POST
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

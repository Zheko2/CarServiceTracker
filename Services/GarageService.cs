using CarServiceTracker.Data;
using CarServiceTracker.Models;
using CarServiceTracker.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CarServiceTracker.Services
{
    public class GarageService : IGarageService
    {
        private readonly ApplicationDbContext _context;

        public GarageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Garage>> GetAllAsync()
        {
            return await _context.Garages
                .Include(g => g.Cars)
                .ToListAsync();
        }

        public async Task<Garage?> GetByIdAsync(int id)
        {
            return await _context.Garages
                .Include(g => g.Cars)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task CreateAsync(Garage garage)
        {
            _context.Garages.Add(garage);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var garage = await _context.Garages.FindAsync(id);

            if (garage != null)
            {
                _context.Garages.Remove(garage);
                await _context.SaveChangesAsync();
            }
        }
    }
}
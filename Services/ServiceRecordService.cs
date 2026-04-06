using CarServiceTracker.Data;
using CarServiceTracker.Models;
using CarServiceTracker.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CarServiceTracker.Services
{
    public class ServiceRecordService : IServiceRecordService
    {
        private readonly ApplicationDbContext _context;

        public ServiceRecordService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceRecord>> GetAllAsync(int? carId, int page, int pageSize)
        {
            var query = _context.ServiceRecords
                .Include(r => r.Car)
                .Include(r => r.ServiceType)
                .AsQueryable();

            if (carId.HasValue)
            {
                query = query.Where(r => r.CarId == carId.Value);
            }

            return await query
                .OrderByDescending(r => r.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(int? carId)
        {
            var query = _context.ServiceRecords.AsQueryable();

            if (carId.HasValue)
            {
                query = query.Where(r => r.CarId == carId.Value);
            }

            return await query.CountAsync();
        }

        public async Task<ServiceRecord?> GetByIdAsync(int id)
        {
            return await _context.ServiceRecords
                .Include(r => r.Car)
                .Include(r => r.ServiceType)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task CreateAsync(ServiceRecord record)
        {
            _context.ServiceRecords.Add(record);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ServiceRecord record)
        {
            _context.ServiceRecords.Update(record);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var record = await _context.ServiceRecords.FindAsync(id);

            if (record != null)
            {
                _context.ServiceRecords.Remove(record);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string?> GetFilterCarNameAsync(int carId)
        {
            var car = await _context.Cars
                .Where(c => c.Id == carId)
                .Select(c => new { c.Brand, c.Model, c.Year })
                .FirstOrDefaultAsync();

            if (car == null)
            {
                return null;
            }

            return $"{car.Brand} {car.Model} ({car.Year})";
        }
    }
}
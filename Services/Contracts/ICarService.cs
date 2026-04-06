using CarServiceTracker.Models;

namespace CarServiceTracker.Services.Contracts
{
    public interface ICarService
    {
        Task<IEnumerable<Car>> GetAllAsync(string? searchTerm);
        Task<Car?> GetByIdAsync(int id);
        Task CreateAsync(Car car);
        Task UpdateAsync(Car car);
        Task DeleteAsync(int id);
    }
}
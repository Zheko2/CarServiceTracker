using CarServiceTracker.Models;

namespace CarServiceTracker.Services.Contracts
{
    public interface IGarageService
    {
        Task<IEnumerable<Garage>> GetAllAsync();
        Task<Garage?> GetByIdAsync(int id);
        Task CreateAsync(Garage garage);
        Task DeleteAsync(int id);
    }
}

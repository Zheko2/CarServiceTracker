using CarServiceTracker.Models;

namespace CarServiceTracker.Services.Contracts
{
    public interface IServiceRecordService
    {
        Task<IEnumerable<ServiceRecord>> GetAllAsync(int? carId, int page, int pageSize);
        Task<int> GetCountAsync(int? carId);
        Task<ServiceRecord?> GetByIdAsync(int id);
        Task CreateAsync(ServiceRecord record);
        Task UpdateAsync(ServiceRecord record);
        Task DeleteAsync(int id);
        Task<string?> GetFilterCarNameAsync(int carId);
    }
}
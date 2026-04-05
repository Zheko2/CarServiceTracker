using CarServiceTracker.Models;

namespace CarServiceTracker.Services.Contracts
{
    public interface IExpenseService
    {
        Task<IEnumerable<Expense>> GetAllAsync();
        Task CreateAsync(Expense expense);
    }
}
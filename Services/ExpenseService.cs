using CarServiceTracker.Data;
using CarServiceTracker.Models;
using CarServiceTracker.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CarServiceTracker.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly ApplicationDbContext _context;

        public ExpenseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Expense>> GetAllAsync()
        {
            return await _context.Expenses
                .Include(e => e.Car)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task CreateAsync(Expense expense)
        {
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
        }
    }
}
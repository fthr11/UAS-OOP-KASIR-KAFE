using kasirkafe.Data;
using kasirkafe.Interfaces;
using kasirkafe.Models;
using Microsoft.EntityFrameworkCore;

namespace kasirkafe.Repositories
{
    public class TransactionService : ITransactionService
    {
        private readonly CafeDbContext _context;

        public TransactionService(CafeDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateTransactionAsync(Transaction transaction)
        {
            try
            {
                // Hitung total dan kembalian jika belum
                if (transaction.TotalAmount == 0)
                {
                    transaction.CalculateTotal();
                }
                if (transaction.ChangeAmount == 0)
                {
                    transaction.CalculateChange();
                }

                // Simpan Transaction beserta TransactionDetails
                // EF Core akan otomatis handle cascade insert
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                return transaction.TransactionId;
            }
            catch (Exception ex)
            {
                // Log error untuk debugging
                Console.WriteLine($"Error in CreateTransactionAsync: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                throw;
            }
        }

        public async Task<List<Transaction>> GetAllTransactionsAsync()
        {
            return await _context.Transactions
                .Include(t => t.TransactionDetails)
                    .ThenInclude(d => d.Product)
                .Include(t => t.User)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<Transaction?> GetTransactionByIdAsync(int id)
        {
            return await _context.Transactions
                .Include(t => t.TransactionDetails)
                    .ThenInclude(d => d.Product)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TransactionId == id);
        }
    }
}
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
            // Hitung total
            transaction.TotalAmount = transaction.TransactionDetails.Sum(d => d.Subtotal);

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return transaction.TransactionId;
        }

        public async Task<List<Transaction>> GetAllTransactionsAsync()
        {
            return await _context.Transactions
                .Include(t => t.TransactionDetails)
                .ThenInclude(d => d.Product)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<Transaction?> GetTransactionByIdAsync(int id)
        {
            return await _context.Transactions
                .Include(t => t.TransactionDetails)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(t => t.TransactionId == id);
       }
    }
}
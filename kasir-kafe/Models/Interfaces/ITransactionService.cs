using kasirkafe.Models;

namespace kasirkafe.Interfaces
{
    public interface ITransactionService
    {
        Task<int> CreateTransactionAsync(Transaction transaction);
        Task<List<Transaction>> GetAllTransactionsAsync();
        Task<Transaction?> GetTransactionByIdAsync(int id);
    }
}